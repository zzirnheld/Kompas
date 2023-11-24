using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Kompas.Server.Gamestate.Players;

namespace Kompas.Server.Effects.Models
{
	public class ServerResolutionContext : ResolutionContext, IServerResolutionContext
	{
		public ServerPlayer ControllingPlayer { get; init; }

		public static ServerResolutionContext PlayerTrigger(Effect effect, IGame game, ServerPlayer controllingPlayer)
			=> new(new TriggeringEventContext(game: game, stackableEvent: effect), controllingPlayer);

		public ServerResolutionContext(TriggeringEventContext? triggerContext, ServerPlayer controllingPlayer)
		: this(triggerContext, controllingPlayer, 0,
			Enumerable.Empty<GameCard>(), default,
			Enumerable.Empty<GameCardInfo>(),
			Enumerable.Empty<Space>(), default,
			Enumerable.Empty<IStackable>(), default)
		{ }

		public ServerResolutionContext(TriggeringEventContext? triggerContext,
			ServerPlayer controllingPlayer, int startIndex,
			IEnumerable<GameCard> cardTargets, GameCard? delayedCardTarget,
			IEnumerable<GameCardInfo> cardInfoTargets,
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