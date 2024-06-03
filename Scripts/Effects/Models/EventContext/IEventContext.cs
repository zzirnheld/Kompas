using Kompas.Cards.Models;

namespace Kompas.Effects.Models.TriggeringEvent
{
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

		public static IEventContext Empty(string triggeringEvent)
			=> new EventContext(triggeringEvent);

		public static EventContextBuilder Build()
			=> Build(Trigger.None);

		/// <summary>
		/// Lets you build up an IEventContext one param at a time.
		/// Prefer this usage, and its Capture function!
		/// HOWEVER be wary that the builder is stateful,
		/// so you'll need to Clone it if you want to reuse builders.
		/// </summary>
		public static EventContextBuilder Build(string triggeringEvent)
			=> new EventContextBuilder(triggeringEvent);
	}
}