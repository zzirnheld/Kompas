using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate;
using Kompas.Server.Gamestate.Players;

namespace Kompas.Server.Effects.Models;

public class ServerResolutionContext : ResolutionContext, IServerResolutionContext
{
	public ServerPlayer ControllingPlayer { get; init; }

	public ServerSubeffect? OnImpossible { get; set; } = null;

	public static ServerResolutionContext PlayerTrigger(IEffect effect, ServerPlayer controllingPlayer)
		=> new(new EventContext() { StackableEvent = effect }, controllingPlayer, effect.InitialBlurb);

	public ServerResolutionContext(IEventContext? triggerContext, ServerPlayer controllingPlayer, string blurb)
	: this(triggerContext, controllingPlayer, 0,
		Enumerable.Empty<GameCard>(),
		Enumerable.Empty<GameCardInfo>(),
		Enumerable.Empty<Space>(),
		Enumerable.Empty<IStackable>(),
		blurb)
	{ }

	public static ServerResolutionContext Resume(IResolutionContext context,
		IEventContext newTriggerContext, ServerPlayer controllingPlayer, int startIndex, string blurb)
	{
		return new ServerResolutionContext(triggerContext: newTriggerContext,
			controllingPlayer, startIndex,
			context.CardTargets,
			context.CardInfoTargets,
			context.SpaceTargets,
			context.StackableTargets, 
			blurb);
	}

	public ServerResolutionContext(IEventContext? triggerContext,
		ServerPlayer controllingPlayer, int startIndex,
		IEnumerable<GameCard> cardTargets,
		IEnumerable<IGameCardInfo> cardInfoTargets,
		IEnumerable<Space> spaceTargets,
		IEnumerable<IStackable> stackableTargets,
		string blurb)
		: base(triggerContext, startIndex,
		cardTargets, cardInfoTargets,
		spaceTargets, stackableTargets,
		blurb)
	{
		ControllingPlayer = controllingPlayer;
	}
}