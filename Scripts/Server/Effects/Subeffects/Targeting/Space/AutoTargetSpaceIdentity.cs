using Kompas.Effects.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models.Restrictions.Gamestate;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Shared.Exceptions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class AutoTargetSpaceIdentityData : SubeffectData
{
	[JsonProperty(Required = Required.Always)]
	public IIdentity<Space>? spaceIdentity;

	[JsonProperty]
	public IRestriction<Space> spaceRestriction = new AlwaysValid();
}

public class AutoTargetSpaceIdentity : ServerSubeffect
{
	private readonly IIdentity<Space> spaceIdentity;
	private readonly IRestriction<Space> spaceRestriction;

	public AutoTargetSpaceIdentity(AutoTargetSpaceIdentityData data) : base(data)
	{
		spaceIdentity = data.spaceIdentity
			?? throw new MissingJSONValueException(nameof(spaceIdentity), this);
		spaceRestriction = data.spaceRestriction;
	}

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);

		spaceIdentity.Initialize(initializationContext: DefaultInitializationContext);
		spaceRestriction.Initialize(initializationContext: DefaultInitializationContext);
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var space = spaceIdentity.From(resolution.Context, resolution.Context);

		if (space == null) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
		if (!spaceRestriction.IsValid(space, resolution.Context)) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

		resolution.AddSpace(space);
		return Task.FromResult(ResolutionInfo.Next);
	}
}