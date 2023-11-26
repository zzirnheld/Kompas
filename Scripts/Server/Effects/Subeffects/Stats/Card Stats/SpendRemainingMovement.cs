using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class SpendRemainingMovement : ServerSubeffect
	{
		public int mult = 1;
		public int div = 1;
		public int mod = 0;

		public override Task<ResolutionInfo> Resolve()
		{
			var card = CardTarget ?? throw new NullCardException(TargetWasNull);
			int toSpend = (card.SpacesCanMove * mult / div) + mod;
			if (toSpend <= 0 || card.SpacesCanMove < toSpend) return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));

			card.SpacesMoved += toSpend;
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}