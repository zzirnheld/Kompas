using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions.Cards;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public interface IResolutionContext
	{
		/// <summary>
		/// Information describing the event that triggered this effect to occur, if any such event happened. (If it's player-triggered, this is null.) 
		/// </summary>
		public TriggeringEventContext? TriggerContext { get; }

		public int StartIndex { get; }
		//public IList<IGameCard> CardTargets { get; }
		public IList<GameCardInfo> CardInfoTargets { get; }
		public IGameCard? DelayedCardTarget { get; }
		public IList<Space> SpaceTargets { get; }
		public Space? DelayedSpaceTarget { get; }
		public IList<IStackable> StackableTargets { get; }
		public IStackable? DelayedStackableTarget { get; }
		public int X { get; set; }

		public static IResolutionContext<TCard, TPlayer> PlayerTrigger<TCard, TPlayer>(IEffect? effect, IGame<TCard, TPlayer> game)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> new ResolutionContext<TCard, TPlayer>(new TriggeringEventContext(game: game, stackableEvent: effect));
	}

	public interface IResolutionContext<TCard, TPlayer> : IResolutionContext
		where TCard : class, IGameCard<TCard, TPlayer>
		where TPlayer : IPlayer<TCard, TPlayer>
	{
		/// <summary>
		/// Use this resolution context to test for a hypothetical resolution
		/// (but only if you expect resolution context to not be referenced)
		/// </summary>
		public static IResolutionContext<TCard, TPlayer> NotResolving
			=> new DummyResolutionContext(null);

		public static IResolutionContext<TCard, TPlayer> Dummy(TriggeringEventContext? triggeringEventContext)
			=> new DummyResolutionContext(triggeringEventContext);
		public IList<TCard> CardTargets { get; }

		public IResolutionContext<TCard, TPlayer> Copy { get; }


		/// <summary>
		/// Used for places that need a resolution context (like triggers calling any other identity), but to enforce never having 
		/// </summary>
		private class DummyResolutionContext : IResolutionContext<TCard, TPlayer>
		{
			private const string NotImplementedMessage = "Dummy resolution context should never have resolution information checked. Use the secondary (aka stashed) resolution context instead.";
			public TriggeringEventContext? TriggerContext { get; }

			public int StartIndex => throw new System.NotImplementedException(NotImplementedMessage);
			public IList<TCard> CardTargets => throw new System.NotImplementedException(NotImplementedMessage);
			//IList<IGameCard> IResolutionContext.CardTargets => throw new System.NotImplementedException(NotImplementedMessage);
			public IList<GameCardInfo> CardInfoTargets => throw new System.NotImplementedException(NotImplementedMessage);
			public IGameCard DelayedCardTarget => throw new System.NotImplementedException(NotImplementedMessage);
			public IList<Space> SpaceTargets => throw new System.NotImplementedException(NotImplementedMessage);
			public Space DelayedSpaceTarget => throw new System.NotImplementedException(NotImplementedMessage);
			public IList<IStackable> StackableTargets => throw new System.NotImplementedException(NotImplementedMessage);
			public IStackable DelayedStackableTarget => throw new System.NotImplementedException(NotImplementedMessage);
			public int X { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

			public IResolutionContext<TCard, TPlayer> Copy => new DummyResolutionContext(TriggerContext);

			public DummyResolutionContext(TriggeringEventContext? triggerContext)
			{
				TriggerContext = triggerContext;
			}
		}
	}
}