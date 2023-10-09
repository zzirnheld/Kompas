using Kompas.Effects.Models;
using Kompas.Effects.Models.Identities.Cards;
using Kompas.Effects.Models.Identities.Numbers;

namespace Kompas.Server.Effects.Subeffects
{
	public class PayPipsTargetCost : PayPips
	{
		public int multiplier = 1;
		public int modifier = 0;
		public int divisor = 1;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			pipCost = new FromCardValue() {
				cardValue = new CardValue() {
					value = CardValue.Cost,
					multiplier = multiplier,
					modifier = modifier,
					divisor = divisor
				},
				card = new TargetIndex()
			};
			base.Initialize(eff, subeffIndex);
		}
	}
}