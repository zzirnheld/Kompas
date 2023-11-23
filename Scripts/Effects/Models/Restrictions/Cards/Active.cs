using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Active : CardRestrictionBase
	{
		[JsonProperty]
		public bool active = true;

		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
			=> card?.Activated == active;
	}
}