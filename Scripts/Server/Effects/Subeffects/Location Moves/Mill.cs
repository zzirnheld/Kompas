using Kompas.Cards.Movement;
using Kompas.Effects.Subeffects;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class MillData : SubeffectData { }

public class Mill : ServerSubeffect
{
	public Mill(MillData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		for (int i = 0; i < AdjustX(resolution.Context); i++)
		{
			var card = GetPlayerTarget(resolution.Context).Deck.Topdeck;
			if (card == null) return Task.FromResult(ResolutionInfo.Impossible(CouldntMillAllX));
			resolution.AddTarget(card);
			card.Discard(ServerEffect);
		}

		return Task.FromResult(ResolutionInfo.Next);
	}
}