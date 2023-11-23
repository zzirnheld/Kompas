using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Damage : ServerSubeffect
	{
		public override bool IsImpossible (TargetingContext? overrideContext = null)
			=> GetCardTarget(overrideContext) == null;

		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && CardTarget.Location != Location.Board)
				throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

			CardTarget.TakeDamage(Count, Effect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}