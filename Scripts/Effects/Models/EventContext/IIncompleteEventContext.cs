using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.TriggeringEvent
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
}