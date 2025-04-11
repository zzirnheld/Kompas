using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTriggeringStackableCard : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		resolution.AddTarget(resolution.Context.TriggerContext?.StackableCause?.Card
			??  throw new KompasException("Null stackable", string.Empty));
		return Task.FromResult(ResolutionInfo.Next);
	}
}