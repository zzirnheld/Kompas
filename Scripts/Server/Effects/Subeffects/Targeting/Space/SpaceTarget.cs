using Kompas.Cards.Models;
using Kompas.Effects.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Effects.Models.Restrictions.Spaces;
using Kompas.Server.Networking;
using Newtonsoft.Json;
using Kompas.Shared.Enumerable;
using Kompas.Effects.Subeffects;
using Kompas.Shared.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SpaceTargetData : SubeffectData
{
	[JsonProperty]
	public string? blurb;
	
	[JsonProperty(Required = Required.Always)]
	public IRestriction<Space>? spaceRestriction;
}

public class SpaceTarget : ServerSubeffect
{
	public string? blurb;
	
	public IRestriction<Space> spaceRestriction;

	public SpaceTarget(SpaceTargetData data) : base(data)
	{
		spaceRestriction = data.spaceRestriction
			?? throw new MissingJSONValueException(nameof(spaceRestriction), this);
		blurb = data.blurb;
	}

	private bool ForPlay => spaceRestriction is AllOf allOf && allOf.elements.Any(elem => elem is CanPlayCard);

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		spaceRestriction.Initialize(DefaultInitializationContext);
	}

	public IEnumerable<Space> GetValidSpaces(IResolutionContext context)
	{
		var player = GetPlayerTarget(context);
		return Space.Spaces
			.Where(s => spaceRestriction.IsValid(s, context))
			.Select(s => player?.SubjectiveCoords(s))
			.NonNull();
	}

	public override bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null)
		=> !GetValidSpaces(context).Any();

	/// <summary>
	/// Whether this space target subeffect will be valid if the given theoretical target is targeted.
	/// </summary>
	/// <param name="theoreticalTarget">The card to theoretically be targeted.</param>
	/// <returns><see langword="true"/> if there's a valid space,
	/// assuming you pick <paramref name="theoreticalTarget"/>,
	/// <see langword="false"/> otherwise</returns>
	public bool WillBePossibleIfCardTargeted(GameCard? theoreticalTarget, IResolutionContext context)
	{
		if (theoreticalTarget == null) return false;
		foreach (var space in Space.Spaces)
		{
			if (Effect.identityOverrides.WithTargetCardOverride(theoreticalTarget,
				() => spaceRestriction.IsValid(space, context)))
				return true;
		}

		return false;
	}

	public override async Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var spaces = GetValidSpaces(resolution.Context).ToArray();
		var recommendedSpaces = ForPlay
			? spaces
				.Where(s => GetCardTarget(resolution.Context)?.PlayRestriction.IsRecommendedPlay((s, GetPlayerTarget(resolution.Context)), resolution.Context)
					?? false)
				.ToArray()
			: spaces;
		if (recommendedSpaces.Length == 0 && spaces.Length != 0)
		{
			Logger.Err($"Recommending 0 spaces! What? The spaces we were gonna allow were {spaces} while {resolution.Context}");
		}
		_ = GetPlayerTarget(resolution.Context) ?? throw new System.InvalidOperationException("Deleted a player target!?");
		if (spaces.Length > 0)
		{
			var space = Space.Invalid;
			while (!SetTargetIfValid(space, resolution))
			{
				space = await ServerGame.Awaiter.GetSpaceTarget
					(GetPlayerTarget(resolution.Context), Effect.Card?.CardName ?? string.Empty, blurb ?? string.Empty, spaces, recommendedSpaces);
				if (space == Space.Invalid && resolution.Context.CanDeclineTarget) return ResolutionInfo.Impossible(DeclinedFurtherTargets);
			}
			return ResolutionInfo.Next;
		}
		else
		{
			Logger.Log($"No valid coords exist for {Effect.Card?.CardName} effect");
			return ResolutionInfo.Impossible(NoValidSpaceTarget);
		}
	}

	public bool SetTargetIfValid(Space space, ServerEffectResolution resolution)
	{
		//evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
		if (space.IsValid && spaceRestriction.IsValid(space, resolution.Context))
		{
			Logger.Log($"Adding {space} as coords");
			resolution.AddSpace(space);
			_ = GetPlayerTarget(resolution.Context) ?? throw new System.InvalidOperationException("Deleted a player target!?");
			ServerNotifier.AcceptTarget(GetPlayerTarget(resolution.Context));
			return true;
		}
		//else Logger.Err($"{x}, {y} not valid for restriction {spaceRestriction}");

		return false;
	}
}