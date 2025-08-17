using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.ManyCards;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models.Restrictions.Gamestate;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Server.Effects.Controllers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ResummonAllData : SubeffectData
{
	[JsonProperty]
	public IRestriction<IGameCardInfo> cardRestriction = new AlwaysValid();

	[JsonProperty]
	public IIdentity<IReadOnlyCollection<IGameCardInfo>>? cards;
	
}

public class ResummonAll : ServerSubeffect
{
	private readonly IIdentity<IReadOnlyCollection<IGameCardInfo>> cards;

	public ResummonAll(ResummonAllData data) : base(data)
	{
		cards ??= new Restricted() { cardRestriction = data.cardRestriction };
	}

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		cards.Initialize(DefaultInitializationContext);
	}

	public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
	{
		base.AdjustSubeffectIndices(increment, startingAtIndex);
		cards.AdjustSubeffectIndices(increment, startingAtIndex);
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		//TODO make a toManyContexts that becomes a wrapper object for an enumerable set of contexts, which can all Capture the same event
		var cardsToEnumerate = cards.From(resolution.Context)
			?.Select(gci => gci.Card)
			?? throw new NullCardException(TargetWasNull);
		foreach (var c in cardsToEnumerate)
		{
			var contexts = IEventContext.Build(Trigger.Play)
				.PrimarilyAffecting(c)
				.CausedBy(Effect)
				.ForPlayer(GetPlayerTarget(resolution.Context))
				.At(c.Position)
				.Capture(() => { },
					ctxt => ctxt,
					ctxt => ctxt.CloneForEvent(Trigger.Arrive));
			ServerGame.StackController.TriggerFor(contexts);
		}

		return Task.FromResult(ResolutionInfo.Next);
	}
}