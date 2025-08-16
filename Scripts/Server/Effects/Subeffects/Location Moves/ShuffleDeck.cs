using System.Threading.Tasks;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ShuffleDeckData : SubeffectData { }

public class ShuffleDeck : ServerSubeffect
{
	public ShuffleDeck(ShuffleDeckData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		GetPlayerTarget(resolution.Context).Deck.Shuffle();
		return Task.FromResult(ResolutionInfo.Next);
	}
}