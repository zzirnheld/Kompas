using KompasCore.GameCore;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class BottomdeckRest : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			DeckController.BottomdeckMany(Effect.rest);

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}