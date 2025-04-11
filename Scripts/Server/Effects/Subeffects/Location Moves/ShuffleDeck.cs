using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ShuffleDeck : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
        GetPlayerTarget(resolution.Context).Deck.Shuffle();
		return Task.FromResult(ResolutionInfo.Next);
	}
}