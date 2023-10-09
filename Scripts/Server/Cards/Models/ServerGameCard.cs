using System.Collections.Generic;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Controllers;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate;

namespace Kompas.Server.Cards.Models
{
	public class ServerGameCard : GameCard
	{
		public ServerGame ServerGame { get; private set; }
		public override IGame Game => ServerGame;

		public ServerEffect[] ServerEffects { get; init; }
		public override IReadOnlyCollection<Effect> Effects => ServerEffects;

		public override bool IsAvatar { get; }

		public override ICardController CardController { get; }

		public override bool KnownToEnemy { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public ServerGameCard(SerializableCard serializeableCard, int id, ServerCardController cardController, IPlayer owningPlayer, ServerEffect[] effects, bool isAvatar)
			: base(serializeableCard, id, owningPlayer)
		{
			CardController = cardController;
			IsAvatar = isAvatar;
		}

		/*
		public ServerCardController ServerCardController { get; private set; }
		public override CardController CardController => ServerCardController;

		public ServerEffectsController EffectsController => ServerGame?.effectsController;
		public ServerNotifier ServerNotifier => ServerController?.notifier;

		private ServerPlayer serverController;
		public ServerPlayer ServerController
		{
			get => serverController;
			set
			{
				serverController = value;
				CardController.SetRotation();
				ServerNotifier.NotifyChangeController(this, ServerController);
				foreach (var eff in Effects) eff.Controller = value;
			}
		}
		public override IPlayer Controller
		{
			get => ServerController;
			set => ServerController = value as ServerPlayer;
		}

		public ServerPlayer ServerOwner { get; private set; }
		public override IPlayer Owner => ServerOwner;

		public override int SpacesMoved {
			get => base.SpacesMoved;
			set {
				bool changed = SpacesMoved != value;
				base.SpacesMoved = value;
				if (changed) ServerNotifier?.NotifySpacesMoved(this);
			}
		}

		public override int AttacksThisTurn {
			get => base.AttacksThisTurn;
			set 
			{
				bool changed = AttacksThisTurn != value;
				base.AttacksThisTurn = value;
				if (changed) ServerNotifier?.NotifyAttacksThisTurn(this);
			}
		}

		public override CardLocation Location
		{
			get => base.Location;
			protected set
			{
				if (Location == CardLocation.Hand && value != CardLocation.Hand && !KnownToEnemy)
					ServerController.enemy.notifier.NotifyDecrementHand();

				if (Location != value) ResetCard();

				base.Location = value;
				switch (Location)
				{
					case CardLocation.Discard:
					case CardLocation.Board:
					case CardLocation.Annihilation:
						KnownToEnemy = true;
						break;
					case CardLocation.Deck:
						KnownToEnemy = false;
						break;
						//Otherwise, KnownToEnemy doesn't change, if it's been added to the hand
						//discard->rehand is public, but deck->rehand is private, for example
				}
			}
		}

		private bool knownToEnemy = false;
		public override bool KnownToEnemy
		{
			get => knownToEnemy;
			set
			{
				bool old = knownToEnemy;
				knownToEnemy = value;
				//update clients if changed
				if (old != value) ServerNotifier.NotifyKnownToEnemy(this, old);
			}
		}

		public ServerGameCard(ServerSerializableCard card, int id, ServerCardController serverCardController, ServerPlayer owner, ServerEffect[] effects)
			: base(card, id, owner.game)
		{
			owner.game.AddCard(this);
			ServerCardController = serverCardController;
			serverCardController.serverCard = this;
			//Don't just grab effects from the card, because that won't include keywords

			ServerEffects = effects;
			ServerGame = owner.game;
			ServerOwner = ServerController = owner;
			foreach (var (index, eff) in effects.Enumerate())
				eff.SetInfo(this, ServerGame, owner, index);

			serverCardController.gameCardViewController.Focus(this);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
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
				GD.Print("Tried to reset card whose info was never set! This should only be the case at game start");
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

		public override void AddAugment(GameCard augment, IStackable stackSrc = null)
		{
			var attachedContext = new TriggeringEventContext(game: ServerGame, CardBefore: augment, secondaryCardBefore: this,
				space: Position, stackableCause: stackSrc, player: stackSrc?.Controller ?? Controller);
			var augmentedContext = new TriggeringEventContext(game: ServerGame, CardBefore: this, secondaryCardBefore: augment,
				space: Position, stackableCause: stackSrc, player: stackSrc?.Controller ?? Controller);
			bool wasKnown = augment.KnownToEnemy;
			base.AddAugment(augment, stackSrc);
			attachedContext.CacheCardInfoAfter();
			augmentedContext.CacheCardInfoAfter();
			EffectsController.TriggerForCondition(Trigger.AugmentAttached, attachedContext);
			EffectsController.TriggerForCondition(Trigger.Augmented, augmentedContext);
			ServerGame.serverPlayers[augment.ControllerIndex].notifier.NotifyAttach(augment, Position, wasKnown);
		}

		protected override void Detach(IStackable stackSrc = null)
		{
			var formerlyAugmentedCard = AugmentedCard;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, secondaryCardBefore: formerlyAugmentedCard,
				stackableCause: stackSrc, player: stackSrc?.Controller ?? Controller);
			base.Detach(stackSrc);
			context.CacheCardInfoAfter();
			EffectsController.TriggerForCondition(Trigger.AugmentDetached, context);
		}

		public override bool Remove(IStackable stackSrc = null)
		{
			//GD.Print($"Trying to remove {CardName} from {Location}");

			//proc the trigger before actually removing anything
			var player = stackSrc?.Controller ?? Controller;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: player);

			var cardsThisLeft = Location == CardLocation.Board ?
				Game.BoardController.CardsAndAugsWhere(c => c != null && c.CardInAOE(this)).ToList() :
				new List<GameCard>();
			var leaveContexts = cardsThisLeft.Select(c =>
				new TriggeringEventContext(game: ServerGame, CardBefore: this, secondaryCardBefore: c, stackableCause: stackSrc, player: player)).ToArray();

			var ret = base.Remove(stackSrc);

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
			return ret;
		}

		public override void Reveal(IStackable stackSrc = null)
		{
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller);
			base.Reveal(stackSrc);
			context.CacheCardInfoAfter();
			EffectsController.TriggerForCondition(Trigger.Revealed, context);
			//logic for actually revealing to client has to happen server-side.
			KnownToEnemy = true;
			ServerController.enemy.notifier.NotifyRevealCard(this);
		}

		#region stats
		public override void SetN(int n, IStackable stackSrc, bool onlyStatBeingSet = true)
		{
			if (n == N) return;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller, x: n - N);
			base.SetN(n, stackSrc);
			context.CacheCardInfoAfter();
			EffectsController?.TriggerForCondition(Trigger.NChange, context);

			if (onlyStatBeingSet) ServerNotifier.NotifyStats(this);
		}

		public override void SetE(int e, IStackable stackSrc = null, bool onlyStatBeingSet = true)
		{
			if (e == E) return;
			int oldE = E;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller, x: e - E);
			base.SetE(e, stackSrc);
			context.CacheCardInfoAfter();
			EffectsController?.TriggerForCondition(Trigger.EChange, context);

			if (onlyStatBeingSet) ServerNotifier.NotifyStats(this);

			//kill if applicable
			GD.Print($"E changed from {oldE} to {E}. Should it die?");
			if (E <= 0 && CardType == 'C' && Summoned && Location != CardLocation.Nowhere && Location != CardLocation.Discard) this.Discard(stackSrc);
		}

		public override void SetS(int s, IStackable stackSrc, bool onlyStatBeingSet = true)
		{
			if (s == S) return;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller, x: s - S);
			base.SetS(s, stackSrc);
			context.CacheCardInfoAfter();
			EffectsController?.TriggerForCondition(Trigger.SChange, context);

			if (onlyStatBeingSet) ServerNotifier.NotifyStats(this);
		}

		public override void SetW(int w, IStackable stackSrc, bool onlyStatBeingSet = true)
		{
			if (w == W) return;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller, x: w - W);
			base.SetW(w, stackSrc);
			context.CacheCardInfoAfter();
			EffectsController?.TriggerForCondition(Trigger.WChange, context);

			if (onlyStatBeingSet) ServerNotifier.NotifyStats(this);
		}

		public override void SetC(int c, IStackable stackSrc, bool onlyStatBeingSet = true)
		{
			if (c == C) return;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller, x: c - C);
			base.SetC(c, stackSrc);
			context.CacheCardInfoAfter();
			EffectsController?.TriggerForCondition(Trigger.CChange, context);

			if (onlyStatBeingSet) ServerNotifier.NotifyStats(this);
		}

		public override void SetA(int a, IStackable stackSrc, bool onlyStatBeingSet = true)
		{
			if (a == A) return;
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller, x: a - A);
			base.SetA(a, stackSrc);
			context.CacheCardInfoAfter();
			EffectsController?.TriggerForCondition(Trigger.AChange, context);

			if (onlyStatBeingSet) ServerNotifier.NotifyStats(this);
		}

		public override void TakeDamage(int dmg, IStackable stackSrc = null)
		{
			int netDmg = dmg;
			base.TakeDamage(netDmg, stackSrc);
		}

		public override void SetCharStats(int n, int e, int s, int w, IStackable stackSrc = null)
		{
			base.SetCharStats(n, e, s, w, stackSrc);
			ServerNotifier.NotifyStats(this);
		}

		public override void SetStats(CardStats stats, IStackable stackSrc = null)
		{
			base.SetStats(stats, stackSrc);
			ServerNotifier?.NotifyStats(this);
		}

		public override void SetNegated(bool negated, IStackable stackSrc = null)
		{
			if (Negated != negated)
			{
				//Notify of value being set to, even if it won't actually change whether the card is negated or not
				//so that the client can know how many negations a card has
				ServerNotifier.NotifySetNegated(this, negated);

				var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller);
				context.CacheCardInfoAfter();
				if (negated) EffectsController.TriggerForCondition(Trigger.Negate, context);
			}
			base.SetNegated(negated, stackSrc);
		}

		public override void SetActivated(bool activated, IStackable stackSrc = null)
		{
			var context = new TriggeringEventContext(game: ServerGame, CardBefore: this, stackableCause: stackSrc, player: stackSrc?.Controller);
			if (Activated != activated)
			{
				//Notify of value being set to, even if it won't actually change whether the card is activated or not,
				//so that the client can know how many activations a card has
				ServerNotifier.NotifyActivate(this, activated);

				context.CacheCardInfoAfter();
				if (activated) EffectsController.TriggerForCondition(Trigger.Activate, context);
				else EffectsController.TriggerForCondition(Trigger.Deactivate, context);
			}
			base.SetActivated(activated, stackSrc);
		}
		#endregion stats
	} */
	}
}