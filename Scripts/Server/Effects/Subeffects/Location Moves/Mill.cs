using Kompas.Cards.Movement;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Mill : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			for (int i = 0; i < Count; i++)
			{
				var card = PlayerTarget.Deck.Topdeck;
				if (card == null) return Task.FromResult(ResolutionInfo.Impossible(CouldntMillAllX));
				ServerEffect.AddTarget(card);
				card.Discard(ServerEffect);
			}

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}