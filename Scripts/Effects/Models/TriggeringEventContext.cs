using System;
using System.Runtime.CompilerServices;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public class TriggeringEventContext
	{
		public readonly IGame game;

		/// <summary>
		/// Information about the primary card involved in the triggering event,
		/// stashed before the triggering event
		/// </summary>
		public GameCardInfo? MainCardInfoBefore { get; }

		/// <summary>
		/// Information about the secondary card involved in the triggering event,
		/// stashed before the triggering event.
		/// The secondary card is often something like "the other card in the attack"
		/// </summary>
		public GameCardInfo? SecondaryCardInfoBefore { get; }

		/// <summary>
		/// The card that caused the triggering event.<br/>
		/// - If an event happened because of an effect, this is the effect's source.<br/>
		/// - If an event is to do with how an attack proceeds, 
		///  like the fight starting or ending, or combat damage being dealt,
		///  the attacker is considered to have caused the attack.<br/>
		/// - If an event happened because of an attack, 
		///  excluding any triggers directly related to the attack itself,
		///  this is the other card involved in the attack.
		///  (Think a character dying during a fight. That was caused by the other card.)
		/// </summary>
		public GameCardInfo? CardCauseBefore { get; }

		/// <summary>
		/// The object on the stack that caused this event to occur.
		/// For example, if an effect caused an attack to start, this would be the effect.
		/// </summary>
		public IStackable? StackableCause { get; }

		/// <summary>
		/// The object on the stack that this trigger describes an event related to.
		/// For example, if this is an "Attack" event, the stackableEvent is that attack.
		/// </summary>
		public IStackable? StackableEvent { get; }

		public IPlayer? Player { get; }
		public int? X { get; }
		public Space? Space { get; }


		/// <summary>
		/// The information for the main triggering card,
		/// stashed immediately after the triggering event occurred.
		/// </summary>
		public GameCardInfo? MainCardInfoAfter { get; private set; }

		/// <summary>
		/// The information for the secondary triggering card,
		/// stashed immediately after the triggering event occurred.
		/// The secondary card could be the defender in an "Attack" trigger, etc.
		/// </summary>
		public GameCardInfo? SecondaryCardInfoAfter { get; private set; }

		/// <summary>
		/// The information for the card that caused the event,
		/// stashed immediately after the triggering event occurred.
		/// </summary>
		public GameCardInfo? CauseCardInfoAfter { get; private set; }

		private readonly string cachedToString;

		private TriggeringEventContext(IGame game,
								  GameCardInfo? mainCardInfoBefore,
								  GameCardInfo? secondaryCardInfoBefore,
								  GameCardInfo? cardCauseBefore,
								  IStackable? stackableCause,
								  IStackable? stackableEvent,
								  IPlayer? player,
								  int? x,
								  Space? space)
		{
			this.game = game;
			this.MainCardInfoBefore = mainCardInfoBefore;
			this.SecondaryCardInfoBefore = secondaryCardInfoBefore;
			this.CardCauseBefore = cardCauseBefore;
			this.StackableCause = stackableCause;
			this.StackableEvent = stackableEvent;
			this.Player = player;
			this.X = x;
			this.Space = space;

			var sb = new System.Text.StringBuilder();

			if (mainCardInfoBefore != null) sb.Append($"Card: {mainCardInfoBefore.CardName}, ");
			if (secondaryCardInfoBefore != null) sb.Append($"Secondary Card: {secondaryCardInfoBefore.CardName}, ");
			if (cardCauseBefore != null) sb.Append($"Card cause: {cardCauseBefore.CardName}, ");
			if (stackableEvent != null) sb.Append($"Stackable Event: {stackableEvent}, ");
			if (stackableCause != null) sb.Append($"Stackable Cause: {stackableCause}, ");
			if (player != null) sb.Append($"Triggerer: {player.Index}, ");
			if (x != null) sb.Append($"X: {x}, ");
			if (space != null) sb.Append($"Space: {space}, ");

			cachedToString = sb.ToString();
		}

		/// <summary>
        /// Constructs a new TriggeringEventContext based on the given data.
        /// Consider using <see cref="Capture"/>,<br/>
        /// or if you don't, remember to <see cref="CacheCardInfoAfter"/>
        /// </summary>
		public TriggeringEventContext(IGame game,
								 GameCard? cardBefore = null, GameCard? secondaryCardBefore = null,
								 GameCard? eventCauseOverride = null,
								 IStackable? stackableCause = null, IStackable? stackableEvent = null,
								 IPlayer? player = null,
								 int? x = null,
								 Space? space = null)
			: this(game: game,
				   mainCardInfoBefore: GameCardInfo.CardInfoOf(cardBefore),
				   secondaryCardInfoBefore: GameCardInfo.CardInfoOf(secondaryCardBefore),
				   //Set the event cause either as the override if one is provided,
				   //or as the stackable's cause if not.
				   cardCauseBefore: GameCardInfo.CardInfoOf(eventCauseOverride ?? stackableCause?.GetCause(cardBefore)),
				   stackableCause: stackableCause,
				   stackableEvent: stackableEvent,
				   player: player,
				   x: x,
				   space: space?.Copy)
		{ }

		/// <summary>
		/// Caches the state of the card(s) relevant to the effect immediately after the triggering event occurred
		/// </summary>
		/// <param name="mainCard">The main card whose information to cache</param>
		/// <param name="secondaryCard">The secondary card whose information to cache, if any 
		/// (like the attacker on a defends proc)</param>
		public void CacheCardInfoAfter()
		{
			if (MainCardInfoBefore != null)
			{
				if (MainCardInfoAfter != null)
					throw new System.ArgumentException("Already initialized MainCardInfoAfter on this context");
				else
					MainCardInfoAfter = GameCardInfo.CardInfoOf(MainCardInfoBefore?.Card);
			}

			if (SecondaryCardInfoBefore != null)
			{
				if (SecondaryCardInfoAfter != null)
					throw new System.ArgumentException("Already initialized SecondaryCardInfoAfter on this context");
				else
					SecondaryCardInfoAfter = GameCardInfo.CardInfoOf(SecondaryCardInfoBefore.Card);
			}

			if (CardCauseBefore != null)
			{
				if (CauseCardInfoAfter != null)
					throw new System.ArgumentException("Already initialized CauseCardInfoAfter on this context");
				else
					CauseCardInfoAfter = GameCardInfo.CardInfoOf(CardCauseBefore.Card);
			}
		}

		public override string ToString() => cachedToString;

		/// <summary>
        /// Usage: start the builder before you take the action,
        /// then call <see cref="ITriggeringEventContextBuilder.CacheToFinalize"/> after the operation
        /// to simultaneously cache the data after the operation, and then return you the finished TriggeringEventContext.
        /// </summary>
        /// <param name="game">The only thing you must provide is a Game</param>
		public static ITriggeringEventContextBuilder BuildContext(IGame game)
			=> new TriggeringEventContextBuilder(game);

		/// <summary>
        /// Usage: start the builder before you take the action,
        /// then call <see cref="CacheToFinalize"/> after the operation
        /// to simultaneously cache the data after the operation, and then return you the finished TriggeringEventContext.
        /// </summary>
        /// <param name="game">The only thing you must provide is a Game</param>
		public interface ITriggeringEventContextBuilder
		{
			public ITriggeringEventContextBuilder PrimarilyAffecting(GameCard mainCard);
			public ITriggeringEventContextBuilder SecondarilyAffecting(GameCard secondaryCard);
			public ITriggeringEventContextBuilder AffectingBoth(GameCard mainCard, GameCard? secondaryCard);
			public ITriggeringEventContextBuilder At(Space? space);
			public ITriggeringEventContextBuilder Affecting(IPlayer? player);
			public ITriggeringEventContextBuilder X(int x);
			public ITriggeringEventContextBuilder CausedBy(GameCard? cardCause);
			public ITriggeringEventContextBuilder CausedBy(IStackable? stackableCause);
			public ITriggeringEventContextBuilder During(IStackable? stackableEvent);

			public ITriggeringEventContextBuilder Clone();

			public TriggeringEventContext CacheToFinalize();
		}

		protected class TriggeringEventContextBuilder
			: ITriggeringEventContextBuilder
		{
			private readonly IGame game;

			private GameCardInfo? mainCardBefore;
			private GameCardInfo? secondaryCardBefore;
			private GameCardInfo? cardCauseBefore;

			private IStackable? stackableCause;
			private IStackable? stackableEvent;

			private IPlayer? player;
			private int? x;
			private Space? space;

			public TriggeringEventContextBuilder(IGame game)
			{
				this.game = game;
			}

			public ITriggeringEventContextBuilder PrimarilyAffecting(GameCard mainCard)
			{
				mainCardBefore = GameCardInfo.CardInfoOf(mainCard);

				return this;
			}

			public ITriggeringEventContextBuilder SecondarilyAffecting(GameCard secondaryCard)
			{
				secondaryCardBefore = GameCardInfo.CardInfoOf(secondaryCard);

				return this;
			}

			public ITriggeringEventContextBuilder AffectingBoth(GameCard mainCard, GameCard? secondaryCard = null)
			{
				mainCardBefore = GameCardInfo.CardInfoOf(mainCard);
				if (secondaryCard != null) secondaryCardBefore = GameCardInfo.CardInfoOf(secondaryCard);

				return this;
			}

			public ITriggeringEventContextBuilder At(Space? space)
			{
				this.space = space;

				return this;
			}

			public ITriggeringEventContextBuilder Affecting(IPlayer? player)
			{
				this.player = player;

				return this;
			}

			public ITriggeringEventContextBuilder CausedBy(GameCard? cardCause)
			{
				cardCauseBefore = GameCardInfo.CardInfoOf(cardCause);

				return this;
			}

			public ITriggeringEventContextBuilder CausedBy(IStackable? stackableCause)
			{
				this.stackableCause = stackableCause;

				return this;
			}

			public ITriggeringEventContextBuilder During(IStackable? stackableEvent)
			{
				this.stackableEvent = stackableEvent;

				return this;
			}

			public ITriggeringEventContextBuilder X(int x)
			{
				this.x = x;

				return this;
			}

			public ITriggeringEventContextBuilder Clone()
			{
				return new TriggeringEventContextBuilder(game)
				{
					mainCardBefore = mainCardBefore,
					secondaryCardBefore = secondaryCardBefore,
					cardCauseBefore = cardCauseBefore,

					stackableCause = stackableCause,
					stackableEvent = stackableEvent,

					player = player,
					x = x,
					space = space,
				};
			}

			public TriggeringEventContext CacheToFinalize()
			{
				var ret = new TriggeringEventContext(game,
					mainCardBefore, secondaryCardBefore, cardCauseBefore,
					stackableCause, stackableEvent, player, x, space);

				ret.CacheCardInfoAfter();
				return ret;
			}
		}
	}
}