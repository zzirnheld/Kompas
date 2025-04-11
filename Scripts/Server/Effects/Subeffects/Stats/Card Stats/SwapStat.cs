using Kompas.Effects.Models;
using Kompas.Effects.Models.Identities.Numbers;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SwapStat : ServerSubeffect
{
	#nullable disable
	[JsonProperty (Required = Required.Always)]
	public CardValue firstTargetStat;
	[JsonProperty (Required = Required.Always)]
	public CardValue secondTargetStat;
	#nullable restore
	public int secondTargetIndex = -2;

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		firstTargetStat.Initialize(DefaultInitializationContext);
		secondTargetStat.Initialize(DefaultInitializationContext);
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var secondTarget = resolution.Context.GetCardTarget(secondTargetIndex);
		if (GetCardTarget(resolution.Context) == null)
			throw new NullCardException(TargetWasNull);
		else if (forbidNotBoard && GetCardTarget(resolution.Context).Location != Location.Board)
			throw new InvalidLocationException(GetCardTarget(resolution.Context).Location, GetCardTarget(resolution.Context), ChangedStatsOfCardOffBoard);

		if (secondTarget == null)
			throw new NullCardException(TargetWasNull);
		else if (forbidNotBoard && GetCardTarget(resolution.Context).Location != Location.Board)
			throw new InvalidLocationException(secondTarget.Location, secondTarget, ChangedStatsOfCardOffBoard);

		var firstStat = firstTargetStat.GetValueOf(GetCardTarget(resolution.Context));
		var secondStat = secondTargetStat.GetValueOf(secondTarget);
		firstTargetStat.SetValueOf(GetCardTarget(resolution.Context), secondStat, Effect);
		secondTargetStat.SetValueOf(secondTarget, firstStat, Effect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}