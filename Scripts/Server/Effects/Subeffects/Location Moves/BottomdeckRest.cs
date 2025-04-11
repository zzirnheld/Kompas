using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class BottomdeckRest : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
        GetPlayerTarget(resolution.Context).Deck.BottomdeckMany(resolution.Context.Rest);

		return Task.FromResult(ResolutionInfo.Next);
	}
}