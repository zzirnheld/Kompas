using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class Damage : ServerSubeffect
{
	public override bool IsImpossible (IResolutionContext context, TargetingContext? overrideContext = null)
		=> context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext)) == null;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (CardTarget == null)
			throw new NullCardException(TargetWasNull);
		else if (forbidNotBoard && CardTarget.Location != Location.Board)
			throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

		CardTarget.TakeDamage(Count, Effect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}