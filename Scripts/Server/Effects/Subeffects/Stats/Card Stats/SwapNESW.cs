using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using Kompas.Shared.Exceptions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SwapNESWData : SubeffectData
{
	[JsonProperty(Required = Required.Always)]
	public int[]? targetIndices;

	[JsonProperty]
	public bool swapN = false;
	[JsonProperty]
	public bool swapE = false;
	[JsonProperty]
	public bool swapS = false;
	[JsonProperty]
	public bool swapW = false;
}

public class SwapNESW : ServerSubeffect
{
	public int[] targetIndices;
	public bool swapN;
	public bool swapE;
	public bool swapS;
	public bool swapW;

	public SwapNESW(SwapNESWData data) : base(data)
	{
		targetIndices = data.targetIndices ?? throw new MissingJSONValueException(nameof(targetIndices), this);

		swapN = data.swapN;
		swapE = data.swapE;
		swapS = data.swapS;
		swapW = data.swapW;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var target1 = resolution.Context.GetCardTarget(targetIndices[0]);
		var target2 = resolution.Context.GetCardTarget(targetIndices[1]);
		if (target1 == null)
			throw new NullCardException(TargetWasNull);
		else if (forbidNotBoard && target1.Location != Location.Board)
			throw new InvalidLocationException(target1.Location, target1, ChangedStatsOfCardOffBoard);

		if (target2 == null)
			throw new NullCardException(TargetWasNull);
		else if (forbidNotBoard && target2.Location != Location.Board)
			throw new InvalidLocationException(target2.Location, target2, ChangedStatsOfCardOffBoard);

		target1.SwapCharStats(target2, swapN, swapE, swapS, swapW);
		return Task.FromResult(ResolutionInfo.Next);
	}
}