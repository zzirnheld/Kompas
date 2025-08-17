using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetDisplacementData : SubeffectData
{
	[JsonProperty]
	public int secondarySpaceIndex = -2;
}

public class TargetDisplacement : ServerSubeffect
{
	private readonly int secondarySpaceIndex;

	public TargetDisplacement(TargetDisplacementData data) : base(data)
	{
		secondarySpaceIndex = data.secondarySpaceIndex;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var primarySpace = GetSpaceTarget(resolution.Context);
		var secondarySpace = resolution.Context.GetSpaceTarget(secondarySpaceIndex);
		if (secondarySpace == null)
			return Task.FromResult(ResolutionInfo.Impossible(NoValidSpaceTarget));

		var displacement = secondarySpace.DisplacementTo(primarySpace);
		Logger.Log($"Displacement from {secondarySpace} to {primarySpace} is {displacement}");

		resolution.AddSpace(displacement);
		return Task.FromResult(ResolutionInfo.Next);
	}
}