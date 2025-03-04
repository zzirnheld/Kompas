using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class Reveal : ServerSubeffect
{
	public override bool IsImpossible (IResolutionContext context, TargetingContext? overrideContext = null)
		=> false //account for null prop
		!= context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext))
			?.KnownToEnemy != false; 

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (GetCardTarget(resolution.Context) == null) throw new NullCardException(TargetWasNull);

        GetCardTarget(resolution.Context).Reveal(Effect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}