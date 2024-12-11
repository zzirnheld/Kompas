using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.TriggeringEvent
{
	public class EventContext
		: IEventContext
	{
		public string TriggeringEvent { get; }

		public EventContext(string triggeringEvent = Trigger.None)
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
}