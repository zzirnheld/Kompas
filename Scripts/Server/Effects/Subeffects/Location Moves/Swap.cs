using KompasCore.Cards;
using KompasCore.Cards.Movement;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class Swap : ServerSubeffect
	{
		public int SecondTargetIndex = -2;
		public GameCard SecondTarget => Effect.GetTarget(SecondTargetIndex);
		public override bool IsImpossible(TargetingContext overrideContext = null)
			=> GetCardTarget(overrideContext) == null || SecondTarget == null;

		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && CardTarget.Location != CardLocation.Board)
				throw new InvalidLocationException(CardTarget.Location, CardTarget, MovedCardOffBoard);

			if (SecondTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && SecondTarget.Location != CardLocation.Board)
				throw new InvalidLocationException(SecondTarget.Location, SecondTarget, MovedCardOffBoard);

			CardTarget.Move(SecondTarget.Position, false, ServerEffect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}