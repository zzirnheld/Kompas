using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Controllers;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using Kompas.Server.Networking;
using Kompas.Shared.Enumerable;

namespace Kompas.Server.Cards.Models
{
	public class ServerGameCard : GameCard
	{
		public ServerGame ServerGame { get; }
		public override IGame Game => ServerGame;

		public ServerEffect[] ServerEffects { get; init; }
		public override IReadOnlyCollection<Effect> Effects => ServerEffects;

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

		public override int SpacesMoved {
			get => base.SpacesMoved;
			set {
				bool changed = SpacesMoved != value;
				base.SpacesMoved = value;
				if (changed) ServerNotifier.NotifySpacesMoved(ControllingPlayer, this);
			}
		}

		public override int AttacksThisTurn {
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

		public ServerGameCard(SerializableCard serializeableCard, int id, IPlayer owningPlayer,
			ServerGame game, ServerCardController cardController, ServerEffect[] effects, bool isAvatar)
			: base(serializeableCard, id, owningPlayer)
		{
			ServerGame = game;
			ServerEffects = effects;
			CardController = cardController;
			IsAvatar = isAvatar;

			foreach (var (index, eff) in effects.Enumerate())
				eff.SetInfo(this, ServerGame, index);
		}

		public ServerStackController EffectsController => ServerGame?.StackController
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
				GD.PushWarning("Tried to reset card whose info was never set! This should only be the case at game start");
				return;
			}

			SetInfo(InitialCardValues);

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

			var attachedContext = new TriggeringEventContext(game: ServerGame, CardBefore: augment, secondaryCardBefore: this,
				space: Position, stackableCause: stackSrc, player: stackSrc?.ControllingPlayer ?? ControllingPlayer);
			var augmentedContext = new TriggeringEventContext(game: ServerGame, CardBefore: this, secondaryCardBefore: augment,
				space: Position, stackableCause: stackSrc, player: stackSrc?.ControllingPlayer ?? ControllingPlayer);
			
			base.AddAugment(augment, stackSrc);
			_ = Position ?? throw new NullSpaceOnBoardException(this);

			attachedContext.CacheCardInfoAfter();
			augmentedContext.CacheCardInfoAfter();
			EffectsController.TriggerForCondition(Trigger.AugmentAttached, attachedContext);
			EffectsController.TriggerForCondition(Trigger.Augmented, augmentedContext);

			ServerNotifier.NotifyAttach(augment.ControllingPlayer, augment, Position, wasKnown);
		}

		protected override void Detach(IStackable? stackSrc = null)
		{
			var formerlyAugmentedCard = AugmentedCard;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, secondaryCardBefore: formerlyAugmentedCard,
				stackableCause: stackSrc, player: stackSrc?.ControllingPlayer ?? ControllingPlayer);
			base.Detach(stackSrc);
			context.CacheCardInfoAfter();
			EffectsController.TriggerForCondition(Trigger.AugmentDetached, context);
		}

		public override void Remove(IStackable? stackSrc = null)
		{
			//GD.Print($"Trying to remove {CardName} from {Location}");

			//proc the trigger before actually removing anything
			var player = stackSrc?.ControllingPlayer ?? ControllingPlayer;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: player);

			var cardsThisLeft = Location == Location.Board ?
				Game.Board.CardsAndAugsWhere(c => c != null && c.CardInAOE(this)).ToList() :
				new List<GameCard>();
			var leaveContexts = cardsThisLeft.Select(c =>
				new TriggeringEventContext(game: ServerGame, CardBefore: this, secondaryCardBefore: c, stackableCause: stackSrc, player: player)).ToArray();

			base.Remove(stackSrc);

			context.CacheCardInfoAfter();
			foreach (var lc in leaveContexts)
			{
				lc.CacheCardInfoAfter();
			}
			EffectsController.TriggerForCondition(Trigger.Remove, context);
			EffectsController.TriggerForCondition(Trigger.LeaveAOE, leaveContexts.ToArray());
			//copy the colleciton  so that you can edit the original
			var augments = Augments.ToArray();
			foreach (var aug in augments) aug.Discard(stackSrc);
		}

		public override void Reveal(IStackable? stackSrc = null)
		{
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.ControllingPlayer);
			base.Reveal(stackSrc);
			context.CacheCardInfoAfter();
			EffectsController.TriggerForCondition(Trigger.Revealed, context);
			//logic for actually revealing to client has to happen server-side.
			KnownToEnemy = true;
			ServerNotifier.NotifyRevealCard(ControllingPlayer.Enemy, this);
		}

		#region stats
		public override void SetN(int n, IStackable? stackSrc, bool onlyStatBeingSet = true)
		{
			if (n == N) return;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.ControllingPlayer, x: n - N);
			base.SetN(n, stackSrc);
			context.CacheCardInfoAfter();
			EffectsController?.TriggerForCondition(Trigger.NChange, context);

			if (onlyStatBeingSet) ServerNotifier.NotifyStats(ControllingPlayer, this);
		}

		public override void SetE(int e, IStackable? stackSrc = null, bool onlyStatBeingSet = true)
		{
			if (e == E) return;
			int oldE = E;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.ControllingPlayer, x: e - E);
			base.SetE(e, stackSrc);
			context.CacheCardInfoAfter();
			EffectsController?.TriggerForCondition(Trigger.EChange, context);

			if (onlyStatBeingSet) ServerNotifier.NotifyStats(ControllingPlayer, this);

			//kill if applicable
			GD.Print($"E changed from {oldE} to {E}. Should it die?");
			if (E <= 0 && CardType == 'C' && Summoned && Location != Location.Nowhere && Location != Location.Discard) this.Discard(stackSrc);
		}

		public override void SetS(int s, IStackable? stackSrc, bool onlyStatBeingSet = true)
		{
			if (s == S) return;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.ControllingPlayer, x: s - S);
			base.SetS(s, stackSrc);
			context.CacheCardInfoAfter();
			EffectsController?.TriggerForCondition(Trigger.SChange, context);

			if (onlyStatBeingSet) ServerNotifier.NotifyStats(ControllingPlayer, this);
		}

		public override void SetW(int w, IStackable? stackSrc, bool onlyStatBeingSet = true)
		{
			if (w == W) return;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.ControllingPlayer, x: w - W);
			base.SetW(w, stackSrc);
			context.CacheCardInfoAfter();
			EffectsController?.TriggerForCondition(Trigger.WChange, context);

			if (onlyStatBeingSet) ServerNotifier.NotifyStats(ControllingPlayer, this);
		}

		public override void SetC(int c, IStackable? stackSrc, bool onlyStatBeingSet = true)
		{
			if (c == C) return;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.ControllingPlayer, x: c - C);
			base.SetC(c, stackSrc);
			context.CacheCardInfoAfter();
			EffectsController?.TriggerForCondition(Trigger.CChange, context);

			if (onlyStatBeingSet) ServerNotifier.NotifyStats(ControllingPlayer, this);
		}

		public override void SetA(int a, IStackable? stackSrc, bool onlyStatBeingSet = true)
		{
			if (a == A) return;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.ControllingPlayer, x: a - A);
			base.SetA(a, stackSrc);
			context.CacheCardInfoAfter();
			EffectsController?.TriggerForCondition(Trigger.AChange, context);

			if (onlyStatBeingSet) ServerNotifier.NotifyStats(ControllingPlayer, this);
		}

		public override void TakeDamage(int dmg, IStackable? stackSrc = null)
		{
			int netDmg = dmg; //Relic of "shield" mechanic. Might bring back sometime, so I'm leaving this
			base.TakeDamage(netDmg, stackSrc);
		}

		public override void SetCharStats(int n, int e, int s, int w, IStackable? stackSrc = null)
		{
			base.SetCharStats(n, e, s, w, stackSrc);
			ServerNotifier.NotifyStats(ControllingPlayer, this);
		}

		public override void SetStats(CardStats stats, IStackable? stackSrc = null)
		{
			base.SetStats(stats, stackSrc);
			ServerNotifier.NotifyStats(ControllingPlayer, this);
		}

		public override void SetNegated(bool negated, IStackable? stackSrc = null)
		{
			if (Negated != negated)
			{
				//Notify of value being set to, even if it won't actually change whether the card is negated or not
				//so that the client can know how many negations a card has
				ServerNotifier.NotifySetNegated(ControllingPlayer, this, negated);

				var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.ControllingPlayer);
				context.CacheCardInfoAfter();
				if (negated) EffectsController.TriggerForCondition(Trigger.Negate, context);
			}
			base.SetNegated(negated, stackSrc);
		}

		public override void SetActivated(bool activated, IStackable? stackSrc = null)
		{
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.ControllingPlayer);
			if (Activated != activated)
			{
				//Notify of value being set to, even if it won't actually change whether the card is activated or not,
				//so that the client can know how many activations a card has
				ServerNotifier.NotifyActivate(ControllingPlayer, this, activated);

				context.CacheCardInfoAfter();
				if (activated) EffectsController.TriggerForCondition(Trigger.Activate, context);
				else EffectsController.TriggerForCondition(Trigger.Deactivate, context);
			}
			base.SetActivated(activated, stackSrc);
		}
		#endregion stats
	}
}