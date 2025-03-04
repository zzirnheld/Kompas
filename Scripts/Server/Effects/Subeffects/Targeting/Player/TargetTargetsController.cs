using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTargetsController : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (CardTarget == null) throw new NullCardException(TargetWasNull);
		Effect.playerTargets.Add(CardTarget.ControllingPlayer);
		return Task.FromResult(ResolutionInfo.Next);
	}
}