using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Active : CardRestrictionBase
	{
		[JsonProperty]
		public bool active = true;

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> card.Activated == active;
	}
}