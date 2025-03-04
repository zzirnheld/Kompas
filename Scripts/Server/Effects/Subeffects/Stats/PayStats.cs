using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class PayStats : ServerSubeffect
{
	public int nMult = 0;
	public int eMult = 0;
	public int sMult = 0;
	public int wMult = 0;

	public int nMod = 0;
	public int eMod = 0;
	public int sMod = 0;
	public int wMod = 0;

	public int N => nMult * Effect.X + nMod;
	public int E => eMult * Effect.X + eMod;
	public int S => sMult * Effect.X + sMod;
	public int W => wMult * Effect.X + wMod;

	public override bool IsImpossible (IResolutionContext context, TargetingContext? overrideContext = null)
	{
		var card = context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext));
		return card == null || card.N < N || card.E < E || card.S < S || card.W < W;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (GetCardTarget(resolution.Context) == null)
			throw new NullCardException(TargetWasNull);
		else if (forbidNotBoard && GetCardTarget(resolution.Context).Location != Location.Board)
			throw new InvalidLocationException(GetCardTarget(resolution.Context).Location, GetCardTarget(resolution.Context), ChangedStatsOfCardOffBoard);

		if (GetCardTarget(resolution.Context).N < N ||
            GetCardTarget(resolution.Context).E < E ||
            GetCardTarget(resolution.Context).S < S ||
            GetCardTarget(resolution.Context).W < W)
			return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));

        GetCardTarget(resolution.Context).AddToCharStats(-1 * N, -1 * E, -1 * S, -1 * W, Effect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}