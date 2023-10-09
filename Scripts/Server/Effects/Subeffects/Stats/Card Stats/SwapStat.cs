using Kompas.Effects.Models;
using Kompas.Effects.Models.Identities.Numbers;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class SwapStat : ServerSubeffect
	{
		public CardValue firstTargetStat;
		public CardValue secondTargetStat;
		public int secondTargetIndex = -2;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			firstTargetStat?.Initialize(DefaultInitializationContext);
			secondTargetStat?.Initialize(DefaultInitializationContext);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			var secondTarget = Effect.GetTarget(secondTargetIndex);
			if (CardTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && CardTarget.Location != Location.Board)
				throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

			if (secondTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && CardTarget.Location != Location.Board)
				throw new InvalidLocationException(secondTarget.Location, secondTarget, ChangedStatsOfCardOffBoard);

			var firstStat = firstTargetStat.GetValueOf(CardTarget);
			var secondStat = secondTargetStat.GetValueOf(secondTarget);
			firstTargetStat.SetValueOf(CardTarget, secondStat, Effect);
			secondTargetStat.SetValueOf(secondTarget, firstStat, Effect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}