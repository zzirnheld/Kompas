using Kompas.Effects.Models.Identities.Cards;
using Kompas.Effects.Models.Identities.Numbers;
using Kompas.Effects.Subeffects;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class PayPipsTargetCostData : SubeffectData
{
	[JsonProperty]
	public int multiplier = 1;
	[JsonProperty]
	public int modifier = 0;
	[JsonProperty]
	public int divisor = 1;
}

public class PayPipsTargetCost : PayPips
{
	public PayPipsTargetCost(PayPipsTargetCostData data) : base(Translate(data)) { }

	private static PayPipsData Translate(PayPipsTargetCostData data) => new()
	{
		pipCost = new FromCardValue()
		{
			cardValue = new CardValue()
			{
				value = CardValue.Cost,
				multiplier = data.multiplier,
				modifier = data.modifier,
				divisor = data.divisor
			},
			card = new TargetIndex()
		}
	};
}