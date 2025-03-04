using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ShuffleDeck : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		PlayerTarget.Deck.Shuffle();
		return Task.FromResult(ResolutionInfo.Next);
	}
}