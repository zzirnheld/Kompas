using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Server.Effects.Models.Subeffects;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class CanMove : CardRestrictionBase
	{
		#nullable disable
		[JsonProperty]
		public IIdentity<Space> destination;
		#nullable restore
		/// <summary>
		/// Index of a subeffect whose space restriction should be considered for whether you'll be able to move this card there.
		/// Be warned! If there's any additional targeting in the meantime (on which the valid movement depends) this might not work as expected.
		/// I'll need to figure out a better solution, if one is possible.
		/// </summary>
		[JsonProperty]
		public int spaceRestrictionSubeffectIndex = int.MinValue;

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			destination?.Initialize(initializationContext);
			if (spaceRestrictionSubeffectIndex != int.MinValue
				&& InitializationContext.effect?.Subeffects[spaceRestrictionSubeffectIndex] is not SpaceTarget)
			{
				throw new System.ArgumentException($"{spaceRestrictionSubeffectIndex} isn't a space target subeffect! for {InitializationContext.effect}");
			}
		}

		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
		{
			if (card == null) return false;
			
			bool IsValidMoveSpace(Space space) => card.MovementRestriction.IsValid(space, context);

			if (destination != null)
			{
				var space = destination.From(context)
					?? throw new System.InvalidOperationException();
				return IsValidMoveSpace(space);
			}
			else if (spaceRestrictionSubeffectIndex != default)
			{
				if (InitializationContext.effect?.Subeffects[spaceRestrictionSubeffectIndex] is not SpaceTarget spaceTargetSubeffect)
				{
					throw new System.ArgumentException($"{spaceRestrictionSubeffectIndex} isn't a space target subeffect! resolving {InitializationContext.effect}");
				}

				bool IsSubeffectRestrictionValid(Space space) => spaceTargetSubeffect.spaceRestriction.IsValid(space, context);
				return InitializationContext.effect.identityOverrides.WithTargetCardOverride(card,
					() => Space.Spaces.Where(IsSubeffectRestrictionValid).Any(IsValidMoveSpace));
			}
			else return Space.Spaces.Any(IsValidMoveSpace);
		}
	}
}