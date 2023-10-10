using Kompas.Cards.Models;
using Kompas.Effects.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Effects.Models.Restrictions.Spaces;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class SpaceTarget : ServerSubeffect
	{
		public string blurb;

		public IRestriction<Space> spaceRestriction;

		private bool ForPlay => spaceRestriction is AllOf allOf && allOf.elements.Any(elem => elem is CanPlayCard);

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			spaceRestriction.Initialize(DefaultInitializationContext);
		}

		public IEnumerable<Space> ValidSpaces => Space.Spaces
				.Where(s => spaceRestriction.IsValid(s, ResolutionContext))
				.Select(s => PlayerTarget.SubjectiveCoords(s));

		public override bool IsImpossible(TargetingContext overrideContext = null)
			=> !ValidSpaces.Any();

		/// <summary>
		/// Whether this space target subeffect will be valid if the given theoretical target is targeted.
		/// </summary>
		/// <param name="theoreticalTarget">The card to theoretically be targeted.</param>
		/// <returns><see langword="true"/> if there's a valid space,
		/// assuming you pick <paramref name="theoreticalTarget"/>,
		/// <see langword="false"/> otherwise</returns>
		public bool WillBePossibleIfCardTargeted(GameCard theoreticalTarget)
		{
			foreach (var space in Space.Spaces)
			{
				if (Effect.identityOverrides.WithTargetCardOverride(theoreticalTarget,
					() => spaceRestriction.IsValid(space, ResolutionContext)))
					return true;
			}

			return false;
		}

		public override async Task<ResolutionInfo> Resolve()
		{
			var spaces = ValidSpaces.Select(s => (s.x, s.y)).ToArray();
			var recommendedSpaces
				= ForPlay
				? spaces.Where(s => CardTarget.PlayRestriction.IsRecommendedPlay((s, PlayerTarget), ResolutionContext)).ToArray()
				: spaces;
			if (spaces.Length > 0)
			{
				var (a, b) = (-1, -1);
				while (!SetTargetIfValid(a, b))
				{
					(a, b) = await ServerGame.Awaiter.GetSpaceTarget(PlayerTarget, Card.CardName, blurb, spaces, recommendedSpaces);
					if ((a, b) == (-1, -1) && ServerEffect.CanDeclineTarget) return ResolutionInfo.Impossible(DeclinedFurtherTargets);
				}
				return ResolutionInfo.Next;
			}
			else
			{
				GD.Print($"No valid coords exist for {Effect.Card.CardName} effect");
				return ResolutionInfo.Impossible(NoValidSpaceTarget);
			}
		}

		public bool SetTargetIfValid(int x, int y)
		{
			Space space = (x, y);
			//evaluate the target. if it's valid, confirm it as the target (that's what the true is for)
			if (space.IsValid && spaceRestriction.IsValid(space, ResolutionContext))
			{
				GD.Print($"Adding {x}, {y} as coords");
				ServerEffect.AddSpace(space);
				ServerNotifier.AcceptTarget(PlayerTarget);
				return true;
			}
			//else GD.PrintErr($"{x}, {y} not valid for restriction {spaceRestriction}");

			return false;
		}
	}
}