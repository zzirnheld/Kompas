using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class PayStats : ServerSubeffect
	{
		public int nMult = 0;
		public int eMult = 0;
		public int sMult = 0;
		public int wMult = 0;

		public int nMod = 0;
		public int eMod = 0;
		public int sMod = 0;
		public int wMod = 0;

		public int N => nMult * Effect.X + nMod;
		public int E => eMult * Effect.X + eMod;
		public int S => sMult * Effect.X + sMod;
		public int W => wMult * Effect.X + wMod;

		public override bool IsImpossible (TargetingContext? overrideContext = null)
		{
			var card = GetCardTarget(overrideContext);
			return CardTarget == null || CardTarget.N < N || CardTarget.E < E || CardTarget.S < S || CardTarget.W < W;
		}

		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && CardTarget.Location != Location.Board)
				throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

			if (CardTarget.N < N ||
				CardTarget.E < E ||
				CardTarget.S < S ||
				CardTarget.W < W)
				return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));

			CardTarget.AddToCharStats(-1 * N, -1 * E, -1 * S, -1 * W, Effect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}