using System.Threading.Tasks;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetThisSpace : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			if (Card.Location != Location.Board)
				return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

			ServerEffect.AddSpace(Card.Position);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}