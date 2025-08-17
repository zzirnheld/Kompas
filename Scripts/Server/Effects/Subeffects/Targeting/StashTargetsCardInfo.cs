using Kompas.Cards.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class StashTargetsCardInfoData : SubeffectData { }

public class StashTargetsCardInfo : ServerSubeffect
{
	public StashTargetsCardInfo(StashTargetsCardInfoData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var targetInfo = GameCardInfo.CardInfoOf(GetCardTarget(resolution.Context))
			?? throw new System.InvalidOperationException("Failed to create a card info!");
		resolution.Context.CardInfoTargets.Add(targetInfo);
		return Task.FromResult(ResolutionInfo.Next);
	}
}
