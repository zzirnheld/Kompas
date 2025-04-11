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

    public int GetN(int x) => nMult * x + nMod;
    public int GetE(int x) => eMult * x + eMod;
    public int GetS(int x) => sMult * x + sMod;
    public int GetW(int x) => wMult * x + wMod;

    public override bool IsImpossible (IResolutionContext context, TargetingContext? overrideContext = null)
	{
		int x = context.X;
		var card = context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext));
		return card == null || card.N < GetN(x) || card.E < GetE(x) || card.S < GetS(x) || card.W < GetW(x);
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (GetCardTarget(resolution.Context) == null)
			throw new NullCardException(TargetWasNull);
		else if (forbidNotBoard && GetCardTarget(resolution.Context).Location != Location.Board)
			throw new InvalidLocationException(GetCardTarget(resolution.Context).Location, GetCardTarget(resolution.Context), ChangedStatsOfCardOffBoard);

		int x = resolution.Context.X;

		if (GetCardTarget(resolution.Context).N < GetN(x) ||
			GetCardTarget(resolution.Context).E < GetE(x) ||
			GetCardTarget(resolution.Context).S < GetS(x) ||
			GetCardTarget(resolution.Context).W < GetW(x))
			return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));

        GetCardTarget(resolution.Context).AddToCharStats(-1 * GetN(x), -1 * GetE(x), -1 * GetS(x), -1 * GetW(x), Effect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}