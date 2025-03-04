using Kompas.Effects.Models;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetDirection : ServerSubeffect
{
	public int secondarySpaceIndex = -2;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var secondarySpace = Effect.GetSpace(secondarySpaceIndex);

		if (SpaceTarget == null || secondarySpace == null)
			return Task.FromResult(ResolutionInfo.Impossible(NoValidSpaceTarget));

		var displacement = secondarySpace.DirectionFromThisTo(SpaceTarget);
		Logger.Log($"Displacement from {secondarySpace} to {SpaceTarget} is {displacement}");

		resolution.AddSpace(displacement);
		return Task.FromResult(ResolutionInfo.Next);
	}
}