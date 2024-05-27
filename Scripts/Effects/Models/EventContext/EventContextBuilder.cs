using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.TriggeringEvent
{
    public class EventContextBuilder
		: IIncompleteEventContext
	{
		public string TriggeringEvent { get; }

		public EventContextBuilder(string triggeringEvent)
		{
			TriggeringEvent = triggeringEvent;
		}

		public IGameCardInfo? MainCardBefore { get; private set; }
		public IGameCardInfo? SecondaryCardBefore { get; private set; }
		public IGameCardInfo? CauseCardBefore { get; private set; }

		public IStackable? StackableEvent { get; private set; }
		public IStackable? StackableCause { get; private set; }

		public IPlayer? Player { get; private set; }
		public int? X { get; private set; }
		public Space? Space { get; private set; }

        private void DetermineCauseCard()
        {
            CauseCardBefore ??= GameCardInfo.CardInfoOf(StackableCause?.GetCause(MainCardBefore));
        }

		public EventContextBuilder PrimarilyAffecting(IGameCardInfo card)
		{
			MainCardBefore = card;
			DetermineCauseCard();
			return this;
		}

		public EventContextBuilder SecondarilyAffecting(IGameCardInfo card)
		{
			SecondaryCardBefore = card;
			return this;
		}

		public EventContextBuilder AffectingBoth(IGameCardInfo primary, IGameCardInfo secondary)
			=> PrimarilyAffecting(primary).SecondarilyAffecting(secondary);

		public EventContextBuilder At(Space? space)
		{
			Space = space;
			return this;
		}

		public EventContextBuilder ForPlayer(IPlayer? player)
		{
			Player = player;
			return this;
		}

		/// <summary>
		/// Prefer using CausedBy(Effect) before, if you want to further override the cause with this function.
		/// </summary>
		public EventContextBuilder CausedBy(GameCard? cardCause)
		{
			CauseCardBefore = GameCardInfo.CardInfoOf(cardCause);
			return this;
		}

		/// <summary>
		/// Prefer using this after PrimarilyAffecting so you can get the cause card w/r/t this effect.
		/// Prefer using CausedBy(GameCard) after, if you want to further override the cause with that function.
		/// </summary>
		public EventContextBuilder CausedBy(IStackable? stackableCause)
        {
            StackableCause = stackableCause;
            DetermineCauseCard();
            return this;
        }

        public EventContextBuilder During(IStackable? stackableEvent)
		{
			StackableEvent = stackableEvent;
			return this;
		}

		public EventContextBuilder WithX(int x)
		{
			X = x;
			return this;
		}

		public EventContextBuilder Clone() => CloneForEvent(TriggeringEvent);

		//TODO unit test: context.Clone().equals(context)
		public EventContextBuilder CloneForEvent(string triggeringEvent) => new(triggeringEvent)
		{
			MainCardBefore = MainCardBefore,
			SecondaryCardBefore = SecondaryCardBefore,
			CauseCardBefore = CauseCardBefore,

			StackableEvent = StackableEvent,
			StackableCause = StackableCause,

			Player = Player,
			X = X,
			Space = Space,
		};

		public IEventContext CaptureNothing() => this.CacheAfterEvent();

        public IReadOnlyCollection<IEventContext> Capture(EventCapturer.CapturableEvent capturableEvent)
			=> EventCapturer.Capture(capturableEvent, this);

		public delegate EventContextBuilder Cloner(EventContextBuilder toClone);

		public IReadOnlyCollection<IEventContext> CaptureAdditionalContexts(EventCapturer.CapturableEvent capturableEvent, IEnumerable<Cloner> cloneOperations)
			=> Capture(capturableEvent, cloneOperations.Prepend(ctx => ctx));

		/// <summary>
		/// Use CaptureAdditionalContexts if you want to include this builder as-is.
		/// </summary>
        public IReadOnlyCollection<IEventContext> Capture(EventCapturer.CapturableEvent capturableEvent, params Cloner[] cloneOperations)
			=> EventCapturer.Capture(capturableEvent,
                cloneOperations.Select(op => op(this)).ToArray());

		public IReadOnlyCollection<IEventContext> Capture(EventCapturer.CapturableEvent capturableEvent, IEnumerable<Cloner> cloneOperations)
			=> EventCapturer.Capture(capturableEvent,
                cloneOperations.Select(op => op(this)).ToArray());
    }
}