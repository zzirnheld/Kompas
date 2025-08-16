using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kompas.Effects.Subeffects;
using Kompas.Shared.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class CanResolveData : SubeffectData
{
	[JsonProperty(Required = Required.Always)]
	public int[]? subeffIndices;

	[JsonProperty]
	public int skipIndex = int.MinValue;

	[JsonProperty]
	public TargetingContext? overrideTargetingContext; //If later necessary, make this an array
}

public class CanResolve : ServerSubeffect
{
	private readonly int[] subeffIndices;
	private readonly int skipIndex;
	private readonly TargetingContext? overrideTargetingContext;

	private IEnumerable<IServerSubeffect> Subeffects => subeffIndices.Select(s => ServerEffect.subeffects[s]);

	public CanResolve(CanResolveData data) : base(data)
	{
		subeffIndices = data.subeffIndices ?? throw new MissingJSONValueException(nameof(subeffIndices), this);

		skipIndex = data.skipIndex;
		overrideTargetingContext = data.overrideTargetingContext;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var impossible = Subeffects.FirstOrDefault(s => s.IsImpossible(resolution.Context, overrideTargetingContext));
		if (impossible == default) return Task.FromResult(ResolutionInfo.Next); //nothing was impossible
		else
		{
			if (skipIndex == int.MinValue) return Task.FromResult(ResolutionInfo.Impossible($"{impossible} couldn't've resolved."));
			else return Task.FromResult(ResolutionInfo.Index(skipIndex));
		}
	}
}