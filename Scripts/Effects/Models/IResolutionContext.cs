using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	//TODO: consider adding ControllingPlayer to IResolutionContext
	public interface IResolutionContext
	{
		/// <summary>
		/// Represents a lack of resolution information.<br/>
        /// Many places, we have the option of passing in a secondary resolution context.<br/>
        /// This can represent many things, usually in the vein of "stashed resolution context",
        /// like for a delayed effect - we want to test against both the triggering event context
        /// + the resolution context when effect resolution was suspended.
        /// Consumers of such restrictions, etc. should only reference the secondary context if they know it's semantically relevant.
		/// </summary>
		public static IResolutionContext Empty
			=> new DummyResolutionContext(null);

		/// <summary>
        /// Wraps a <see cref="TriggeringEventContext"/>,
        /// representing triggering conditions existing even while an effect is not currently resolving.
        /// Used to be able to pass into restrictions that could be checked when an event is resolving,
        /// or while a player does something via other gamerules. (i.e. play by effect vs play by normal gamerule)<br/>
        /// <see cref="CanResolve"/> will be false for <see cref="IResolutionContext"/>s created in this way.<br/>
        /// FUTURE: split out the places that take a resolution context into taking a resolution context + a triggering event context,
        /// so it's semantically obvious that the triggering event context can be present even if there is no actual resolution context
        /// </summary>
        /// <param name="triggeringEventContext"></param>
        /// <returns></returns>
		public static IResolutionContext NotResolving(TriggeringEventContext? triggeringEventContext)
			=> new DummyResolutionContext(triggeringEventContext);

		/// <summary>
        /// Represents a player taking an action that an Effect might otherwise cause.
        /// Ex: playing a card.
        /// We have to pass in an IResolutionContext to restrictions that might want to check details about how it happened,
        /// so we wrap a TriggeringEventContext that simply says a player did it normally.
        /// </summary>
		public static IResolutionContext PlayerAction(IPlayer agent)
			=> NotResolving(new(agent.Game, player: agent));

		/// <summary>
		/// Information describing the event that triggered this effect to occur, if any such event happened. (If it's player-triggered, this is null.) 
		/// </summary>
		public TriggeringEventContext? TriggerContext { get; }

		public int StartIndex { get; }
		public IList<GameCard> CardTargets { get; }
		public IList<GameCardInfo> CardInfoTargets { get; }
		public GameCard? DelayedCardTarget { get; }
		public IList<Space> SpaceTargets { get; }
		public Space? DelayedSpaceTarget { get; }
		public IList<IStackable> StackableTargets { get; }
		public IStackable? DelayedStackableTarget { get; }
		public int X { get; set; }

		public IResolutionContext Copy { get; }

		public bool CanResolve { get; }


		/// <summary>
		/// Used for places that need a resolution context (like triggers calling any other identity), but to enforce never having 
		/// </summary>
		private class DummyResolutionContext : IResolutionContext
		{
			private const string NotImplementedMessage = "Dummy resolution context should never have resolution information checked. Use the secondary (aka stashed) resolution context instead.";
			public TriggeringEventContext? TriggerContext { get; }

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

			public bool CanResolve => false;

			public DummyResolutionContext(TriggeringEventContext? triggerContext)
			{
				TriggerContext = triggerContext;
			}
		}
	}
}