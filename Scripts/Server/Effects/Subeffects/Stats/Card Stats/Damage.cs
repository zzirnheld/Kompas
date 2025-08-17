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
		var card = GetCardTarget(resolution.Context);
		if (forbidNotBoard && card.Location != Location.Board)
			throw new InvalidLocationException(card.Location, card, ChangedStatsOfCardOffBoard);

		card.TakeDamage(AdjustX(resolution.Context), Effect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}