using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class BottomdeckRest : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			PlayerTarget.Deck.BottomdeckMany(Effect.rest);

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}