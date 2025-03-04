using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;
using Kompas.Gamestate.Locations;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class Swap : ServerSubeffect
{
	public int SecondTargetIndex = -2;
	public GameCard SecondTarget => Effect.GetCardTarget(SecondTargetIndex) ?? throw new NullCardException(TargetWasNull);
	public override bool IsImpossible (IResolutionContext context, TargetingContext? overrideContext = null)
		=> context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext)) == null || SecondTarget == null;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (CardTarget == null)
			throw new NullCardException(TargetWasNull);
		if (forbidNotBoard && CardTarget.Location != Location.Board)
			throw new InvalidLocationException(CardTarget.Location, CardTarget, MovedCardOffBoard);
		if (CardTarget.Position == null)
			throw new NullSpaceOnBoardException(CardTarget);

		if (SecondTarget == null)
			throw new NullCardException(TargetWasNull);
		if (SecondTarget.Location != Location.Board)
			throw new InvalidLocationException(SecondTarget.Location, SecondTarget, MovedCardOffBoard);
		if (SecondTarget.Position == null)
			throw new NullSpaceOnBoardException(SecondTarget);

		CardTarget.Move(SecondTarget.Position, false, PlayerTarget, ServerEffect,
			Kompas.Gamestate.Space.ShortestPathBetween(CardTarget.Position, SpaceTarget, 
				space => CardTarget.MovementRestriction.IsValid(space, resolution.Context)));
		return Task.FromResult(ResolutionInfo.Next);
	}
}