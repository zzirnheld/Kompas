using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Negated : CardRestrictionBase
	{
		[JsonProperty]
		public bool negated = true;

		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
			=> card.Negated == negated;
	}
}