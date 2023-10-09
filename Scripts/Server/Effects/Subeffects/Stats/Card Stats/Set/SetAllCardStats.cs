using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.ManyCards;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models.Restrictions.Cards;
using System.Collections.Generic;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class SetAllCardStats : SetCardStatsOld
	{
		public IRestriction<IGameCard> cardRestriction = new Character();

		public IIdentity<IReadOnlyCollection<IGameCard>> cardsCard = new Board();

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			cards ??= new Restricted() {
				cardRestriction = cardRestriction,
				cards = cardsCard
			};
			base.Initialize(eff, subeffIndex);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction.AdjustSubeffectIndices(increment, startingAtIndex);
		}
	}
}