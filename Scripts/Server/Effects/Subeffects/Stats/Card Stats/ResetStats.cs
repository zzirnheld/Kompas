using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class ResetStats : ServerSubeffect
	{
		public bool resetN = false;
		public bool resetE = false;
		public bool resetS = false;
		public bool resetW = false;
		public bool resetC = false;
		public bool resetA = false;

		public override Task<ResolutionInfo> Resolve()
		{
			if (resetN) CardTarget.SetN(CardTarget.BaseN, Effect);
			if (resetE) CardTarget.SetE(CardTarget.BaseE, Effect);
			if (resetS) CardTarget.SetS(CardTarget.BaseS, Effect);
			if (resetW) CardTarget.SetW(CardTarget.BaseW, Effect);
			if (resetC) CardTarget.SetC(CardTarget.BaseC, Effect);
			if (resetA) CardTarget.SetA(CardTarget.BaseA, Effect);

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}