using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate;
using Kompas.Server.Gamestate.Players;

namespace Kompas.Server.Effects.Models
{
	public class ServerResolutionContext : ResolutionContext, IServerResolutionContext
	{
		public ServerPlayer ControllingPlayer { get; init; }

		public static ServerResolutionContext PlayerTrigger(IEffect effect, IGame game, ServerPlayer controllingPlayer)
			=> new(new EventContext() { StackableEvent = effect }, controllingPlayer);

		public ServerResolutionContext(IEventContext? triggerContext, ServerPlayer controllingPlayer)
		: this(triggerContext, controllingPlayer, 0,
			Enumerable.Empty<GameCard>(), default,
			Enumerable.Empty<GameCardInfo>(),
			Enumerable.Empty<Space>(), default,
			Enumerable.Empty<IStackable>(), default)
		{ }

		public static ServerResolutionContext Resume(IResolutionContext context,
			IEventContext newTriggerContext, ServerPlayer controllingPlayer, int startIndex)
		{
			return new ServerResolutionContext(triggerContext: newTriggerContext,
				controllingPlayer, startIndex,
				context.CardTargets, default,
				context.CardInfoTargets,
				context.SpaceTargets, default,
				context.StackableTargets, default);
		}

		public ServerResolutionContext(IEventContext? triggerContext,
			ServerPlayer controllingPlayer, int startIndex,
			IEnumerable<GameCard> cardTargets, GameCard? delayedCardTarget,
			IEnumerable<IGameCardInfo> cardInfoTargets,
			IEnumerable<Space> spaceTargets, Space? delayedSpaceTarget,
			IEnumerable<IStackable> stackableTargets, IStackable? delayedStackableTarget)
			: base (triggerContext,
			startIndex,
			cardTargets, delayedCardTarget,
			cardInfoTargets,
			spaceTargets, delayedSpaceTarget,
			stackableTargets, delayedStackableTarget)
		{
			ControllingPlayer = controllingPlayer;
		}
	}
}