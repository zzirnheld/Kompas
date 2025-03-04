using Kompas.Effects.Models;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetDisplacement : ServerSubeffect
{
	public int secondarySpaceIndex = -2;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var secondarySpace = Effect.GetSpace(secondarySpaceIndex);

		if (GetSpaceTarget(resolution.Context) == null || secondarySpace == null)
			return Task.FromResult(ResolutionInfo.Impossible(NoValidSpaceTarget));

		var displacement = secondarySpace.DisplacementTo(GetSpaceTarget(resolution.Context));
		Logger.Log($"Displacement from {secondarySpace} to {GetSpaceTarget(resolution.Context)} is {displacement}");

		resolution.AddSpace(displacement);
		return Task.FromResult(ResolutionInfo.Next);
	}
}