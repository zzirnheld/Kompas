using Kompas.Effects.Models.Identities.Cards;
using Kompas.Effects.Models.Identities.ManyCards;
using Kompas.Effects.Models.Restrictions.Gamestate;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetAugmentsData : TargetAllData { }

public class TargetAugments : TargetAll
{
	public TargetAugments(TargetAugmentsData data) : base(PopulateToSearch(data)) { }

	private static TargetAllData PopulateToSearch(TargetAugmentsData data)
	{
		data.toSearch = new Restricted()
		{
			cardRestriction = data.cardRestriction ?? new AlwaysValid(),
			cards = new Kompas.Effects.Models.Identities.ManyCards.Augments() { card = new TargetIndex() }
		};
		return data;
	}
}