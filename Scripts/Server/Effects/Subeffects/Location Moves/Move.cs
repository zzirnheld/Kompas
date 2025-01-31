using Kompas.Cards.Movement;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class Move : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve()
	{
		if (CardTarget == null) throw new NullCardException(TargetWasNull);
		if (CardTarget.Position == null) throw new NullSpaceOnBoardException(CardTarget);

		CardTarget.Move(SpaceTarget, false, PlayerTarget, Effect,
			Kompas.Gamestate.Space.ShortestPathBetween(CardTarget.Position, SpaceTarget, 
				space => CardTarget.MovementRestriction.IsValid(space, ResolutionContext)));
		return Task.FromResult(ResolutionInfo.Next);
	}
}