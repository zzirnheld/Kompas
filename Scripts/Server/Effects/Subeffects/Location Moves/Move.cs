using Kompas.Cards.Movement;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class MoveData : SubeffectData { }

public class Move : ServerSubeffect
{
	public Move(MoveData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var card = GetCardTarget(resolution.Context);
		if (card.Position == null) throw new NullSpaceOnBoardException(card);

		var space = GetSpaceTarget(resolution.Context);
		var player = GetPlayerTarget(resolution.Context);
		var path = Kompas.Gamestate.Space.ShortestPathBetween(card.Position, space,
				s => card.MovementRestriction.IsValid(s, resolution.Context));

		card.Move(space, false, player, Effect, path);
		return Task.FromResult(ResolutionInfo.Next);
	}
}