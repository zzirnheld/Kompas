using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Subeffects;
using Kompas.Shared.Exceptions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ConditionalEndData : SubeffectData
{
	[JsonProperty(Required = Required.Always)]
	public IGamestateRestriction? endIfTrue;
}

public class ConditionalEnd : ServerSubeffect
{
	private readonly IGamestateRestriction endIfTrue;

	public ConditionalEnd(ConditionalEndData data) : base(data)
	{
		endIfTrue = data.endIfTrue ?? throw new MissingJSONValueException(nameof(endIfTrue), this);
	}

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		endIfTrue.Initialize(DefaultInitializationContext);
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		//TODO implement a ToHumanReadableString sort of thing to provide as a reason here
		if (endIfTrue.IsValid(resolution.Context)) return Task.FromResult(ResolutionInfo.End("I said so"));
		else return Task.FromResult(ResolutionInfo.Next);
	}
}