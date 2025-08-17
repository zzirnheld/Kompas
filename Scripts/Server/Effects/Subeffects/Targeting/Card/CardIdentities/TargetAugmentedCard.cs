using Kompas.Effects.Models.Identities.Cards;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetAugmentedCardData : AutoTargetCardIdentityData { }

public class TargetAugmentedCard : AutoTargetCardIdentity
{
	public TargetAugmentedCard(TargetAugmentedCardData data) : base(PopulateIdentity(data)) { }

	private static AutoTargetCardIdentityData PopulateIdentity(TargetAugmentedCardData data)
	{
		data.subeffectCardIdentity = new AugmentedCard() { ofThisCard = new ThisCardNow() };
		return data;
	}
}