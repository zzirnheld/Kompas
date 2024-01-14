using Kompas.Effects.Models.Identities.Cards;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetAugmentedCard : AutoTargetCardIdentity
	{
		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			subeffectCardIdentity = new AugmentedCard() { ofThisCard = new ThisCardNow() };
			base.Initialize(eff, subeffIndex);
		}
	}
}