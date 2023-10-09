using KompasCore.Cards;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.ManyCards;
using Kompas.Effects.Models.Restrictions.CardRestrictionElements;
using System.Collections.Generic;

namespace Kompas.Server.Effects.Subeffects
{
	public class ChangeAllCardStats : ChangeCardStats
	{
		//default to making sure things are characters before changing their stats
		public IRestriction<GameCardBase> cardRestriction = new Character();

		public IIdentity<IReadOnlyCollection<GameCardBase>> cardsSource = new Board();


		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			cards ??= new Restricted() {
				cardRestriction = cardRestriction,
				cards = cardsSource
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