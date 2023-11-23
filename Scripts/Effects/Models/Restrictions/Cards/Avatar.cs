using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Avatar : CardRestrictionBase
	{
		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
			=> card.IsAvatar;
	}

	public class Summoned : CardRestrictionBase
	{
		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
			=> card.Summoned;
	}
}