using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class ShuffleDeck : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			PlayerTarget.deckCtrl.Shuffle();
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}