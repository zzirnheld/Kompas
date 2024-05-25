using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public interface IIncompleteEventContext
	{
		public string TriggeringEvent { get; }

		/// <summary>
		/// The main card involved in whatever just happened.
		/// Could be the only card, could be the attacker, etc.
		/// Stashed immediately <b>before</b> the event in question happened.
		/// </summary>
		public IGameCardInfo? MainCardBefore { get; }
		
		/// <summary>
		/// The secondary card involved in whatever just happened.
		/// Usually something like a defender in an attack, or a card swapped with, or the AOE that you left.
		/// Stashed immediately <b>before</b> the event in question happened.
		/// </summary>
		public IGameCardInfo? SecondaryCardBefore { get; }

		/// <summary>
		/// The card that caused whatever just happened to happen.
		/// Stashed immediately <b>before</b> the event in question happened.
		/// </summary>
		public IGameCardInfo? CauseCardBefore { get; }

		/// <summary>
		/// The event being described.
		/// Ex: The attack that just started for an "Attack" trigger.
		/// </summary>
		public IStackable? StackableEvent { get; }
		/// <summary>
		/// The event that caused this to occur
		/// Ex: The attack that caused someone to lose E.
		/// Ex: The effect that caused the attack to occur.
		/// </summary>
		public IStackable? StackableCause { get; }

		public IPlayer? Player { get; }
		public int? X { get; }
		public Space? Space { get; }
	}

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

		public EventContextBuilder PrimarilyAffecting(IGameCardInfo card)
		{
			MainCardBefore = card;
			return this;
		}

		public EventContextBuilder SecondarilyAffecting(IGameCardInfo card)
		{
			SecondaryCardBefore = card;
			return this;
		}

		public EventContextBuilder AffectingBoth(IGameCardInfo primary, IGameCardInfo secondary)
			=> PrimarilyAffecting(primary).SecondarilyAffecting(secondary);

		public EventContextBuilder At(Space space)
		{
			Space = space;
			return this;
		}

		public EventContextBuilder ForPlayer(IPlayer? player)
		{
			Player = player;
			return this;
		}

		public EventContextBuilder CausedBy(GameCard? cardCause)
		{
			CauseCardBefore = GameCardInfo.CardInfoOf(cardCause);
			return this;
		}

		public EventContextBuilder CausedBy(IStackable? stackableCause)
		{
			StackableCause = stackableCause;
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
    }

	public static class IncompleteEventContextExtensions
	{
		public static IEventContext CacheAfterEvent(this IIncompleteEventContext incomplete) => new EventContext(incomplete.TriggeringEvent)
		{
			MainCardBefore = incomplete.MainCardBefore,
			MainCardAfter = incomplete.MainCardBefore?.Now(),

			SecondaryCardBefore = incomplete.SecondaryCardBefore,
			SecondaryCardAfter = incomplete.SecondaryCardBefore?.Now(),

			CauseCardBefore = incomplete.CauseCardBefore,
			CauseCardAfter = incomplete.CauseCardBefore?.Now(),

			StackableEvent = incomplete.StackableEvent,
			StackableCause = incomplete.StackableCause,

			Player = incomplete.Player,
			X = incomplete.X,
			Space = incomplete.Space,
		};
	}

	/// <summary>
	/// Describes an event that occurred.
	/// Should be a superset of the information in IIncompleteEventContext,
	/// but also including copies of anything that needs to be stashed after the event completes.
	/// </summary>
    public interface IEventContext
		: IIncompleteEventContext
	{
		/// <summary>
		/// The main card involved in whatever just happened.
		/// Could be the only card, could be the attacker, etc.
		/// Stashed immediately <b>after</b> the event in question happened.
		/// </summary>
		public IGameCardInfo? MainCardAfter { get; }
		
		/// <summary>
		/// The secondary card involved in whatever just happened.
		/// Usually something like a defender in an attack, or a card swapped with, or the AOE that you left.
		/// Stashed immediately <b>after</b> the event in question happened.
		/// </summary>
		public IGameCardInfo? SecondaryCardAfter { get; }

		/// <summary>
		/// The card that caused whatever just happened to happen.
		/// Stashed immediately <b>after</b> the event in question happened.
		/// </summary>
		public IGameCardInfo? CauseCardAfter { get; }

		public static EventContextBuilder Build(string triggeringEvent)
			=> new EventContextBuilder(triggeringEvent);
	}

    public class EventContext
        : IEventContext
    {
		public string TriggeringEvent { get; }

        public EventContext(string triggeringEvent)
        {
            TriggeringEvent = triggeringEvent;
        }

        public IGameCardInfo? MainCardBefore { get; init; }
        public IGameCardInfo? MainCardAfter { get; init; }

        public IGameCardInfo? SecondaryCardBefore { get; init; }
        public IGameCardInfo? SecondaryCardAfter { get; init; }

        public IGameCardInfo? CauseCardBefore { get; init; }
        public IGameCardInfo? CauseCardAfter { get; init; }

        public IStackable? StackableEvent { get; init; }
        public IStackable? StackableCause { get; init; }

        public IPlayer? Player { get; init; }
        public int? X { get; init; }
        public Space? Space { get; init; }
    }

	public class EventCapturer
	{
		private readonly IReadOnlyCollection<IIncompleteEventContext> incompletes;

        public EventCapturer(IEnumerable<IIncompleteEventContext> incompletes)
        {
            this.incompletes = incompletes.ToArray();
        }

		public delegate void CapturableEvent();

		public static IReadOnlyCollection<IEventContext> Capture(IEnumerable<IIncompleteEventContext> incompletes, CapturableEvent capturableEvent)
		{
			var capturer = new EventCapturer(incompletes);
			return capturer.Capture(capturableEvent);
		}

        public static IReadOnlyCollection<IEventContext> Capture(CapturableEvent capturableEvent, params IIncompleteEventContext[] incompletes)
			=> Capture(incompletes, capturableEvent);

        public IReadOnlyCollection<IEventContext> Capture(CapturableEvent capturableEvent)
		{
			capturableEvent();
			return incompletes
				.Select(incomplete => incomplete.CacheAfterEvent())
				.ToArray();
		}
	}
}