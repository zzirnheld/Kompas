using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class SwapNESW : ServerSubeffect
	{
		public int[] targetIndices;

		public bool swapN = false;
		public bool swapE = false;
		public bool swapS = false;
		public bool swapW = false;

		public override Task<ResolutionInfo> Resolve()
		{
			var target1 = Effect.GetTarget(targetIndices[0]);
			var target2 = Effect.GetTarget(targetIndices[1]);
			if (target1 == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && target1.Location != Location.Board)
				throw new InvalidLocationException(target1.Location, target1, ChangedStatsOfCardOffBoard);

			if (target2 == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && target2.Location != Location.Board)
				throw new InvalidLocationException(target2.Location, target2, ChangedStatsOfCardOffBoard);

			target1.SwapCharStats(target2, swapN, swapE, swapS, swapW);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}