using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Subeffects;
using Kompas.Shared.Exceptions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class AutoTargetCardIdentityData : SubeffectData
{
	[JsonProperty] //Can be populated by inheritors, not always required
	public IIdentity<IGameCardInfo>? subeffectCardIdentity;
	
}

public class AutoTargetCardIdentity : ServerSubeffect
{
	public IIdentity<IGameCardInfo> subeffectCardIdentity;

	public AutoTargetCardIdentity(AutoTargetCardIdentityData data) : base(data)
	{
		subeffectCardIdentity = data.subeffectCardIdentity
			?? throw new MissingJSONValueException(nameof(subeffectCardIdentity), this);
	}

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		subeffectCardIdentity.Initialize(initializationContext: DefaultInitializationContext);
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var card = subeffectCardIdentity.From(resolution.Context, resolution.Context);
		if (card == null) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

		resolution.AddTarget(card.Card);
		return Task.FromResult(ResolutionInfo.Next);
	}
}