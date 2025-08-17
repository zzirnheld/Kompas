using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Cards;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTargetsAugmentedCardData : AutoTargetCardIdentityData
{
	[JsonProperty]
	public IIdentity<IGameCardInfo> card = new TargetIndex();
}

public class TargetTargetsAugmentedCard : AutoTargetCardIdentity
{
	public TargetTargetsAugmentedCard(TargetTargetsAugmentedCardData data) : base(PopulateIdentity(data)) { }

	private static AutoTargetCardIdentityData PopulateIdentity(TargetTargetsAugmentedCardData data)
	{
		data.subeffectCardIdentity = new AugmentedCard() { ofThisCard = data.card };;
		return data;
	}
}