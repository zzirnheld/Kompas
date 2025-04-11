using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

/// <summary>
/// Swaps two values among one card's own NESW. E for W, for example.
/// </summary>
public class SwapOwnNESW : ServerSubeffect
{
	public int Stat1;
	public int Stat2;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (GetCardTarget(resolution.Context) == null)
			throw new NullCardException(TargetWasNull);
		else if (forbidNotBoard && GetCardTarget(resolution.Context).Location != Location.Board)
			throw new InvalidLocationException(GetCardTarget(resolution.Context).Location, GetCardTarget(resolution.Context), ChangedStatsOfCardOffBoard);

		int[] newStats = { GetCardTarget(resolution.Context).N, GetCardTarget(resolution.Context).E, GetCardTarget(resolution.Context).S, GetCardTarget(resolution.Context).W };
		(newStats[Stat1], newStats[Stat2]) = (newStats[Stat2], newStats[Stat1]);
        GetCardTarget(resolution.Context).SetCharStats(newStats[0], newStats[1], newStats[2], newStats[3]);

		return Task.FromResult(ResolutionInfo.Next);
	}
}