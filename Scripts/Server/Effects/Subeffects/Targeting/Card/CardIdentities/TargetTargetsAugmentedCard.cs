using KompasCore.Cards;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Cards;

namespace Kompas.Server.Effects.Subeffects
{
	public class TargetTargetsAugmentedCard : AutoTargetCardIdentity
	{
		public IIdentity<GameCardBase> card = new TargetIndex();

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			subeffectCardIdentity = new AugmentedCard() { ofThisCard = card };
			base.Initialize(eff, subeffIndex);
		}
	}
}