﻿using Kompas.Effects.Models.Identities.Cards;
using Kompas.Effects.Models.Identities.ManyCards;
using Kompas.Effects.Models.Restrictions.Gamestate;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetAugments : TargetAll
	{
		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			toSearch = new Restricted() {
				cardRestriction = cardRestriction ?? new AlwaysValid(),
				cards = new Kompas.Effects.Models.Identities.ManyCards.Augments() { card = new TargetIndex() } };
			base.Initialize(eff, subeffIndex);
		}
	}
}