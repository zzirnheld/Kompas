using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.InitializationRequirements;
using Newtonsoft.Json;

//TODO: move this to the KompasServer package?
//If I do, I'd probably want to have some tyupe of "server-only restriction" thing, where it just always returns true if it's client side.
//That could create some crustiness if I ever for some reason want to check a CardRestriction client side that includes this,
//but considering it should only ever be part of a subeffect, that shouldn't happen.
//worth considering, tho
namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class SubeffectValidIfTargeted : CardRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public int[] subeffectIndices;

		protected override IEnumerable<IInitializationRequirement> InitializationRequirements
			{ get { yield return new SubeffectInitializationRequirement(); } }

		private bool ValidateAllSubeffectsPossible()
		{
			if (InitializationContext.effect is not ServerEffect serverEffect)
				throw new System.InvalidOperationException("Cannot check validity of a server-reliant restriction client-side!");

			return subeffectIndices.Select(i => serverEffect.subeffects[i])
				.All(subeff => !subeff.IsImpossible());
		}

		protected override bool IsValidLogic(IGameCard card, IResolutionContext context)
			=> InitializationContext.effect.TestWithCardTarget(card as GameCard, ValidateAllSubeffectsPossible);

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			AdjustSubeffectIndices(subeffectIndices, increment, startingAtIndex);
		}
	}
}