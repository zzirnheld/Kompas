using Kompas.Cards.Movement;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class Move : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		if (GetCardTarget(resolution.Context) == null) throw new NullCardException(TargetWasNull);
		if (GetCardTarget(resolution.Context).Position == null) throw new NullSpaceOnBoardException(GetCardTarget(resolution.Context));

        GetCardTarget(resolution.Context).Move(GetSpaceTarget(resolution.Context), false, GetPlayerTarget(resolution.Context), Effect,
			Kompas.Gamestate.Space.ShortestPathBetween(GetCardTarget(resolution.Context).Position, GetSpaceTarget(resolution.Context), 
				space => GetCardTarget(resolution.Context).MovementRestriction.IsValid(space, resolution.Context)));
		return Task.FromResult(ResolutionInfo.Next);
	}
}