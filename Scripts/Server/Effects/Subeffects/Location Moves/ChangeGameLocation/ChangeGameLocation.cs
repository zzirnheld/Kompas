using System.Threading.Tasks;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects;

/// <summary>
/// Moves cards between discard/field/etc
/// </summary>
public abstract class ChangeGameLocation : ServerSubeffect
{
	public override bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null)
	{
		var currLocation = context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext))
			?.Location; //TODO allow moving from ex. one hand to another. needs to somehow be aware of which location will end up in
		return currLocation == null || currLocation == Destination;
	}

	protected abstract Location Destination { get; }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
        GameCard target = GetCardTarget(resolution.Context)
			?? throw new NullCardException(TargetWasNull);

        ChangeLocation(target, resolution.Context);
		return Task.FromResult(ResolutionInfo.Next);
	}

	protected abstract void ChangeLocation(GameCard card, IServerResolutionContext context);
}