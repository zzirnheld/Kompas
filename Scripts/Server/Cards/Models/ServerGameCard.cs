using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate;
using Kompas.Server.Networking;
using Kompas.Shared.Enumerable;

namespace Kompas.Server.Cards.Models;

public class ServerGameCard : GameCard
{
	public IServerGame ServerGame { get; }
	public override IGame Game => ServerGame;

	public IServerEffect[] ServerEffects { get; init; }
	public override IReadOnlyCollection<IEffect> Effects => ServerEffects;

	public override bool IsAvatar { get; }

	public override ICardController CardController { get; }

	private bool knownToEnemy = false;
	public override bool KnownToEnemy
	{
		get => knownToEnemy;
		set
		{
			bool old = knownToEnemy;
			knownToEnemy = value;
			//update clients if changed
			if (old != value) ServerNotifier.NotifyKnownToEnemy(ControllingPlayer, this, old);
		}
	}

	public override int SpacesMoved
	{
		get => base.SpacesMoved;
		set
		{
			bool changed = SpacesMoved != value;
			base.SpacesMoved = value;
			if (changed) ServerNotifier.NotifySpacesMoved(ControllingPlayer, this);
		}
	}

	public override int AttacksThisTurn
	{
		get => base.AttacksThisTurn;
		set
		{
			bool changed = AttacksThisTurn != value;
			base.AttacksThisTurn = value;
			if (changed) ServerNotifier.NotifyAttacksThisTurn(ControllingPlayer, this);
		}
	}

	public override Location Location
	{
		get => base.Location;
		protected set
		{
			if (Location == Location.Hand && value != Location.Hand && !KnownToEnemy)
				ServerNotifier.NotifyDecrementHand(ControllingPlayer.Enemy);

			if (Location != value) ResetCard();

			base.Location = value;
			switch (Location)
			{
				case Location.Discard:
				case Location.Board:
				case Location.Annihilation:
					KnownToEnemy = true;
					break;
				case Location.Deck:
					KnownToEnemy = false;
					break;
					//Otherwise, KnownToEnemy doesn't change, if it's been added to the hand
					//discard->rehand is public, but deck->rehand is private, for example
			}
		}
	}

	private ServerGameCard(SerializableCard serializeableCard, int id, IPlayer owningPlayer,
		IServerGame game, ICardController cardController, IServerEffect[] effects, bool isAvatar)
		: base(serializeableCard, id, owningPlayer, game.CardRepository)
	{
		ServerGame = game;
		ServerEffects = effects;
		CardController = cardController;
		IsAvatar = isAvatar;
	}

	public static ServerGameCard Create(SerializableCard serializeableCard, int id, IPlayer owningPlayer,
		IServerGame game, ICardController cardController, IServerEffect[] effects, bool isAvatar)
	{
		var ret = new ServerGameCard(serializeableCard, id, owningPlayer,
			game, cardController, effects, isAvatar);

		foreach (var (index, eff) in effects.Enumerate())
		{
			eff.SetInfo(ret, game, index);
			eff.EffectInformationChanged += (_, _) => ret.RefreshEffectInformation();
		}

		ret.UpdateBBCodeEffectText();

		return ret;
	}

	public IServerStackController EffectsController => ServerGame?.StackController
		?? throw new System.NullReferenceException("Didn't init server game or its stack controller");

	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.Append(base.ToString());
		if (null != Effects)
		{
			foreach (var eff in Effects)
			{
				sb.Append(eff.ToString());
				sb.Append(", ");
			}
		}
		return sb.ToString();
	}

	/// <summary>
	/// Resets any of the card's values that might be different from their originals.
	/// Should be called when cards move out the discard, or into the hand, deck, or annihilation
	/// </summary>
	public void ResetCard()
	{
		if (InitialCardValues == null)
		{
			//TODO make an exception, see if it works anyway
			Logger.Warn("Tried to reset card whose info was never set! This should only be the case at game start");
			return;
		}

		SetInfo(InitialCardValues, Game.CardRepository);

		TurnsOnBoard = 0;
		SpacesMoved = 0;
		AttacksThisTurn = 0;

		if (Effects != null) foreach (var eff in Effects) eff.Reset();
		//instead of setting negations or activations to 0, so that it updates the client correctly
		while (Negated) SetNegated(false);
		while (Activated) SetActivated(false);
	}

	public override void AddAugment(GameCard augment, IStackable? stackSrc = null)
	{
		bool wasKnown = augment.KnownToEnemy;

		var contexts = IEventContext.Build()
			.At(Position)
			.CausedBy(stackSrc)
			.ForPlayer(stackSrc?.ControllingPlayer ?? ControllingPlayer)
			.Capture(() => base.AddAugment(augment, stackSrc),
				ctx => ctx.CloneForEvent(Trigger.AugmentAttached).AffectingBoth(augment, this),
				ctx => ctx.CloneForEvent(Trigger.Augmented).AffectingBoth(this, augment));

		EffectsController.TriggerFor(contexts);

		_ = Position ?? throw new NullSpaceOnBoardException(this);
		ServerNotifier.NotifyAttach(augment.ControllingPlayer, augment, Position, wasKnown);
	}

	protected override void Detach(GameCard augment, IStackable? stackSrc = null)
	{
		var contexts = IEventContext.Build(Trigger.AugmentDetached)
			.AffectingBoth(augment, this)
			.CausedBy(stackSrc)
			.ForPlayer(stackSrc?.ControllingPlayer ?? ControllingPlayer)
			.Capture(() => base.Detach(augment, stackSrc));

		EffectsController.TriggerFor(contexts);
	}

	public override void Remove(IStackable? stackSrc = null)
	{
		//Logger.Log($"Trying to remove {CardName} from {Location}");

		var cardsThisLeft = Location == Location.Board ?
			Game.Board.CardsAndAugsWhere(c => c != null && c.CardInAOE(this)).ToList() :
			new List<GameCard>();

		EventContextBuilder.Cloner ToLeaveContext(GameCard card)
			=> ctx => ctx.CloneForEvent(Trigger.LeaveAOE).SecondarilyAffecting(card);

		var contexts = IEventContext.Build(Trigger.Remove)
			.CausedBy(stackSrc)
			.ForPlayer(stackSrc?.ControllingPlayer ?? ControllingPlayer)
			.PrimarilyAffecting(this)
			.CaptureAdditionalContexts(() => base.Remove(stackSrc),
				cardsThisLeft.Select(ToLeaveContext));

		EffectsController.TriggerFor(contexts);

		//copy the colleciton  so that you can edit the original
		var augments = Augments.ToArray();
		foreach (var aug in augments) aug.Discard(stackSrc);
	}

	public override void Reveal(IStackable? stackSrc = null)
	{
		var contexts = IEventContext.Build(Trigger.Revealed)
			.PrimarilyAffecting(this)
			.CausedBy(stackSrc)
			.ForPlayer(stackSrc?.ControllingPlayer)
			.Capture(() => base.Reveal(stackSrc));
		EffectsController.TriggerFor(contexts);

		//logic for actually revealing to client has to happen server-side.
		ServerNotifier.NotifyRevealCard(ControllingPlayer.Enemy, this);
	}

	#region stats
	public override void SetN(int newN, IStackable? stackSrc, bool onlyStatBeingSet = true)
	{
		if (newN == N) return;

		var contexts = IEventContext.Build(Trigger.NChange)
			.PrimarilyAffecting(this)
			.CausedBy(stackSrc)
			.ForPlayer(stackSrc?.ControllingPlayer)
			.WithX(newN - N)
			.Capture(() => base.SetN(newN, stackSrc));
		EffectsController.TriggerFor(contexts);
	}

	public override void SetE(int newE, IStackable? stackSrc = null, bool onlyStatBeingSet = true)
	{
		if (newE == E) return;

		var contexts = IEventContext.Build(Trigger.EChange)
			.PrimarilyAffecting(this)
			.CausedBy(stackSrc)
			.ForPlayer(stackSrc?.ControllingPlayer)
			.WithX(newE - E)
			.Capture(() => base.SetE(newE, stackSrc));
		EffectsController.TriggerFor(contexts);

		//kill if applicable
		if (E <= 0 && CardType == 'C' && Summoned && Location != Location.Nowhere && Location != Location.Discard) this.Discard(stackSrc);
	}

	public override void SetS(int newS, IStackable? stackSrc, bool onlyStatBeingSet = true)
	{
		if (newS == S) return;

		var contexts = IEventContext.Build(Trigger.SChange)
			.PrimarilyAffecting(this)
			.CausedBy(stackSrc)
			.ForPlayer(stackSrc?.ControllingPlayer)
			.WithX(newS - S)
			.Capture(() => base.SetS(newS, stackSrc));
		EffectsController.TriggerFor(contexts);
	}

	public override void SetW(int newW, IStackable? stackSrc, bool onlyStatBeingSet = true)
	{
		if (newW == W) return;

		var contexts = IEventContext.Build(Trigger.WChange)
			.PrimarilyAffecting(this)
			.CausedBy(stackSrc)
			.ForPlayer(stackSrc?.ControllingPlayer)
			.WithX(newW - W)
			.Capture(() => base.SetW(newW, stackSrc));
		EffectsController.TriggerFor(contexts);
	}

	public override void SetC(int newC, IStackable? stackSrc, bool onlyStatBeingSet = true)
	{
		if (newC == C) return;

		var contexts = IEventContext.Build(Trigger.CChange)
			.PrimarilyAffecting(this)
			.CausedBy(stackSrc)
			.ForPlayer(stackSrc?.ControllingPlayer)
			.WithX(newC - C)
			.Capture(() => base.SetC(newC, stackSrc));
		EffectsController.TriggerFor(contexts);
	}

	public override void SetA(int newA, IStackable? stackSrc, bool onlyStatBeingSet = true)
	{
		if (newA == A) return;

		var contexts = IEventContext.Build(Trigger.AChange)
			.PrimarilyAffecting(this)
			.CausedBy(stackSrc)
			.ForPlayer(stackSrc?.ControllingPlayer)
			.WithX(newA - A)
			.Capture(() => base.SetA(newA, stackSrc));
		EffectsController.TriggerFor(contexts);

		if (onlyStatBeingSet) ServerNotifier.NotifyStats(ControllingPlayer, this);
	}

	public override void TakeDamage(int dmg, IStackable? stackSrc = null)
	{
		int netDmg = dmg; //Relic of "shield" mechanic. Might bring back sometime, so I'm leaving this
		base.TakeDamage(netDmg, stackSrc);
	}

	protected override void OnStatChangeOperations()
	{
		base.OnStatChangeOperations();
		ServerNotifier.NotifyStats(ControllingPlayer, this);
	}

	public override void SetNegated(bool negated, IStackable? stackSrc = null)
	{
		bool changed = Negated != negated;
		//Only trigger effets if go from unnegated to negated
		if (negated && changed)
		{
			var contexts = IEventContext.Build(Trigger.Negate)
				.PrimarilyAffecting(this)
				.CausedBy(stackSrc)
				.ForPlayer(stackSrc?.ControllingPlayer)
				.Capture(() => base.SetNegated(negated, stackSrc));
			EffectsController.TriggerFor(contexts);
		}
		else base.SetNegated(negated, stackSrc);

		//Notify of value being set to, even if it won't actually change whether the card is negated or not
		//so that the client can know how many negations a card has
		if (changed) ServerNotifier.NotifySetNegated(ControllingPlayer, this, negated);
	}

	public override void SetActivated(bool activated, IStackable? stackSrc = null)
	{
		bool changed = Activated != activated;
		//Triggers need to be aware of activating OR deactivating
		if (changed)
		{
			var contexts = IEventContext.Build(activated ? Trigger.Activate : Trigger.Deactivate)
				.PrimarilyAffecting(this)
				.CausedBy(stackSrc)
				.ForPlayer(stackSrc?.ControllingPlayer)
				.Capture(() => base.SetActivated(activated, stackSrc));
			EffectsController.TriggerFor(contexts);
		}
		else base.SetActivated(activated, stackSrc);

		if (changed) ServerNotifier.NotifyActivate(ControllingPlayer, this, activated);
	}
	#endregion stats
}