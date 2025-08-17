using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class DamageData : SubeffectData { }

public class Damage : ServerSubeffect
{
	public Damage(DamageData data) : base(data) { }

	public override bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null)
		=> context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext)) == null;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (GetCardTarget(resolution.Context) == null)
			throw new NullCardException(TargetWasNull);
		else if (forbidNotBoard && GetCardTarget(resolution.Context).Location != Location.Board)
			throw new InvalidLocationException(GetCardTarget(resolution.Context).Location, GetCardTarget(resolution.Context), ChangedStatsOfCardOffBoard);

		GetCardTarget(resolution.Context).TakeDamage(AdjustX(resolution.Context), Effect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}