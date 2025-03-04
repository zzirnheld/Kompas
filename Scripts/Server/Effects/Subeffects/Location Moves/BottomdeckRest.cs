using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class BottomdeckRest : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		PlayerTarget.Deck.BottomdeckMany(resolution.Context.rest);

		return Task.FromResult(ResolutionInfo.Next);
	}
}