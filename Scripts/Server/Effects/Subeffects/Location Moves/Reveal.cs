using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class RevealData : SubeffectData { }

public class Reveal : ServerSubeffect
{
	public Reveal(RevealData data) : base(data) { }

	public override bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null)
		=> false //account for null prop
		!= context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext))
			?.KnownToEnemy != false;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var card = GetCardTarget(resolution.Context)
			?? throw new NullCardException(TargetWasNull);
		card.Reveal(Effect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}