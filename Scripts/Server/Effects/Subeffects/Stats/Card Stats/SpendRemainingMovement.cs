using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class SpendRemainingMovement : ServerSubeffect
	{
		public int mult = 1;
		public int div = 1;
		public int mod = 0;

		public override Task<ResolutionInfo> Resolve()
		{
			int toSpend = (CardTarget.SpacesCanMove * mult / div) + mod;
			if (toSpend <= 0 || CardTarget.SpacesCanMove < toSpend) return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));

			CardTarget.SpacesMoved += toSpend;
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}