using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;
using Kompas.Gamestate.Locations;
using Kompas.Cards.Movement;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Swap : ServerSubeffect
	{
		public int SecondTargetIndex = -2;
		public GameCard SecondTarget => Effect.GetTarget(SecondTargetIndex);
		public override bool IsImpossible (TargetingContext? overrideContext = null)
			=> GetCardTarget(overrideContext) == null || SecondTarget == null;

		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && CardTarget.Location != Location.Board)
				throw new InvalidLocationException(CardTarget.Location, CardTarget, MovedCardOffBoard);

			if (SecondTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && SecondTarget.Location != Location.Board)
				throw new InvalidLocationException(SecondTarget.Location, SecondTarget, MovedCardOffBoard);

			CardTarget.Move(SecondTarget.Position, false, PlayerTarget, ServerEffect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}