using Kompas.Effects.Models;
using KompasCore.Exceptions;
using System.Threading.Tasks;
using Godot;

namespace Kompas.Server.Effects.Subeffects
{
	public class TargetDisplacement : ServerSubeffect
	{
		public int secondarySpaceIndex = -2;

		public override Task<ResolutionInfo> Resolve()
		{
			var secondarySpace = Effect.GetSpace(secondarySpaceIndex);

			if (SpaceTarget == null || secondarySpace == null)
				return Task.FromResult(ResolutionInfo.Impossible(NoValidSpaceTarget));

			var displacement = secondarySpace.DisplacementTo(SpaceTarget);
			GD.Print($"Displacement from {secondarySpace} to {SpaceTarget} is {displacement}");

			Effect.AddSpace(displacement);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}