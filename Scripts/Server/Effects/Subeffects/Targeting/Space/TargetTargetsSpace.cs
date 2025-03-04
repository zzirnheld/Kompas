using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTargetsSpace : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (GetCardTarget(resolution.Context)?.Location != Location.Board)
			return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

		if (GetCardTarget(resolution.Context).Position == null) throw new NullSpaceOnBoardException(GetCardTarget(resolution.Context));

		resolution.AddSpace(GetCardTarget(resolution.Context).Position.Copy);
		return Task.FromResult(ResolutionInfo.Next);
	}
}