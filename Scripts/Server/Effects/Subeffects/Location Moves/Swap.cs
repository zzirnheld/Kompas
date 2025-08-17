using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;
using Kompas.Gamestate.Locations;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SwapData : SubeffectData
{
	[JsonProperty]
	public int secondTargetIndex = -2;
}

public class Swap : ServerSubeffect
{
	private readonly int secondTargetIndex = -2;

	public Swap(SwapData data) : base(data)
	{
		secondTargetIndex = data.secondTargetIndex;
	}

	public GameCard GetSecondTarget(IResolutionContext context)
		=> context.GetCardTarget(secondTargetIndex)
			?? throw new NullCardException(TargetWasNull);

	public override bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null)
		=> context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext)) == null || GetSecondTarget(context) == null;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var firstTarget = GetCardTarget(resolution.Context);
		if (forbidNotBoard && firstTarget.Location != Location.Board)
			throw new InvalidLocationException(firstTarget.Location, firstTarget, MovedCardOffBoard);
		if (firstTarget.Position == null)
			throw new NullSpaceOnBoardException(firstTarget);

		var secondTarget = GetSecondTarget(resolution.Context);
		if (secondTarget.Location != Location.Board)
			throw new InvalidLocationException(secondTarget.Location, secondTarget, MovedCardOffBoard);
		if (secondTarget.Position == null)
			throw new NullSpaceOnBoardException(secondTarget);

		firstTarget.Move(secondTarget.Position, false, GetPlayerTarget(resolution.Context), ServerEffect,
			Kompas.Gamestate.Space.ShortestPathBetween(firstTarget.Position, secondTarget.Position,
				space => firstTarget.MovementRestriction.IsValid(space, resolution.Context)));
		return Task.FromResult(ResolutionInfo.Next);
	}
}