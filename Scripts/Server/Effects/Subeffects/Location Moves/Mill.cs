using Kompas.Cards.Movement;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class Mill : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		for (int i = 0; i < Count; i++)
		{
			var card = GetPlayerTarget(resolution.Context).Deck.Topdeck;
			if (card == null) return Task.FromResult(ResolutionInfo.Impossible(CouldntMillAllX));
			resolution.AddTarget(card);
			card.Discard(ServerEffect);
		}

		return Task.FromResult(ResolutionInfo.Next);
	}
}