using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Negated : CardRestrictionElement
	{
		[JsonProperty]
		public bool negated = true;

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> card.Negated == negated;
	}
}