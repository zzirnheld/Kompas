using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects
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
			var card = CardTarget ?? throw new NullCardException(TargetWasNull);
			if (resetN) card.SetN(card.BaseN, Effect);
			if (resetE) card.SetE(card.BaseE, Effect);
			if (resetS) card.SetS(card.BaseS, Effect);
			if (resetW) card.SetW(card.BaseW, Effect);
			if (resetC) card.SetC(card.BaseC, Effect);
			if (resetA) card.SetA(card.BaseA, Effect);

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}