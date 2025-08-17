using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTriggeringCoordsData : SubeffectData { }

public class TargetTriggeringCoords : ServerSubeffect
{
	public TargetTriggeringCoords(TargetTriggeringCoordsData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.AddSpace(resolution.Context.TriggerContext?.Space
			?? throw new InvalidSpaceException(null, NoValidSpaceTarget));
		return Task.FromResult(ResolutionInfo.Next);
	}
}