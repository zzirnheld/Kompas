using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;
using Godot;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetDirection : ServerSubeffect
	{
		public int secondarySpaceIndex = -2;

		public override Task<ResolutionInfo> Resolve()
		{
			var secondarySpace = Effect.GetSpace(secondarySpaceIndex);

			if (SpaceTarget == null || secondarySpace == null)
				return Task.FromResult(ResolutionInfo.Impossible(NoValidSpaceTarget));

			var displacement = secondarySpace.DirectionFromThisTo(SpaceTarget);
			GD.Print($"Displacement from {secondarySpace} to {SpaceTarget} is {displacement}");

			Effect.AddSpace(displacement);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}