using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTriggeringCoords : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.AddSpace(resolution.Context.TriggerContext?.Space
			?? throw new InvalidSpaceException(null, NoValidSpaceTarget));
		return Task.FromResult(ResolutionInfo.Next);
	}
}