using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class StashTargetsCardInfo : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (GetCardTarget(resolution.Context) == null) throw new NullCardException(NoValidCardTarget);

		var targetInfo = GameCardInfo.CardInfoOf(GetCardTarget(resolution.Context))
			?? throw new System.InvalidOperationException("Failed to create a card info!");
		resolution.Context.CardInfoTargets.Add(targetInfo);
		return Task.FromResult(ResolutionInfo.Next);
	}
}
