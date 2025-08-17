using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.ManyCards;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models.Restrictions.Cards;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ChangeAllCardStatsData : ChangeCardStatsData
{
	//default to making sure things are characters before changing their stats
	[JsonProperty]
	public IRestriction<IGameCardInfo> cardRestriction = new Character();

	[JsonProperty]
	public IIdentity<IReadOnlyCollection<IGameCardInfo>> cardsCard = new Board();
}

public class ChangeAllCardStats : ChangeCardStats
{
	public ChangeAllCardStats(ChangeAllCardStatsData data) : base(PopulateCards(data)) { }

	private static ChangeCardStatsData PopulateCards(ChangeAllCardStatsData data)
	{
		data.cards ??= new Restricted()
		{
			cardRestriction = data.cardRestriction,
			cards = data.cardsCard
		};
		return data;
	}
}