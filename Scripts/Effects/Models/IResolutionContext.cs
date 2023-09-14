using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate;

namespace Kompas.Effects.Models
{
	public interface IResolutionContext
	{
		public static IResolutionContext Dummy(TriggeringEventContext triggeringEventContext)
			=> new DummyResolutionContext(triggeringEventContext);

		/// <summary>
		/// Information describing the event that triggered this effect to occur, if any such event happened. (If it's player-triggered, this is null.) 
		/// </summary>
		public TriggeringEventContext TriggerContext { get; }

		public int StartIndex { get; }
		public IList<GameCard> CardTargets { get; }
		public IList<GameCardInfo> CardInfoTargets { get; }
		public GameCard DelayedCardTarget { get; }
		public IList<Space> SpaceTargets { get; }
		public Space DelayedSpaceTarget { get; }
		public IList<IStackable> StackableTargets { get; }
		public IStackable DelayedStackableTarget { get; }
		public int X { get; set; }

		public IResolutionContext Copy { get; }


		/// <summary>
		/// Used for places that need a resolution context (like triggers calling any other identity), but to enforce never having 
		/// </summary>
		private class DummyResolutionContext : IResolutionContext
		{
			private const string NotImplementedMessage = "Dummy resolution context should never have resolution information checked. Use the secondary (aka stashed) resolution context instead.";
			public TriggeringEventContext TriggerContext { get; }

			public int StartIndex => throw new System.NotImplementedException(NotImplementedMessage);
			public IList<GameCard> CardTargets => throw new System.NotImplementedException(NotImplementedMessage);
			public IList<GameCardInfo> CardInfoTargets => throw new System.NotImplementedException(NotImplementedMessage);
			public GameCard DelayedCardTarget => throw new System.NotImplementedException(NotImplementedMessage);
			public IList<Space> SpaceTargets => throw new System.NotImplementedException(NotImplementedMessage);
			public Space DelayedSpaceTarget => throw new System.NotImplementedException(NotImplementedMessage);
			public IList<IStackable> StackableTargets => throw new System.NotImplementedException(NotImplementedMessage);
			public IStackable DelayedStackableTarget => throw new System.NotImplementedException(NotImplementedMessage);
			public int X { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

			public IResolutionContext Copy => new DummyResolutionContext(TriggerContext);

			public DummyResolutionContext(TriggeringEventContext triggerContext)
			{
				TriggerContext = triggerContext;
			}
		}
	}
}