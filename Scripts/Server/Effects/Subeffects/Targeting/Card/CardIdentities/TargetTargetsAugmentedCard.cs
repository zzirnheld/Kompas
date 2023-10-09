using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Cards;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetTargetsAugmentedCard : AutoTargetCardIdentity
	{
		public IIdentity<IGameCard> card = new TargetIndex();

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			subeffectCardIdentity = new AugmentedCard() { ofThisCard = card };
			base.Initialize(eff, subeffIndex);
		}
	}
}