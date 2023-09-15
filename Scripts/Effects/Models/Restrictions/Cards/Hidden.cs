using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Hidden : CardRestrictionBase
	{
		[JsonProperty]
		public bool hidden = true;

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> card.KnownToEnemy == !hidden;
	}
}