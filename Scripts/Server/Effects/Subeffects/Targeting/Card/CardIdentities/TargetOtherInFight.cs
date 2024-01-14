using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Cards;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetOtherInFight : AutoTargetCardIdentity
	{
		public IIdentity<IGameCardInfo> other = new TargetIndex();

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			subeffectCardIdentity = new OtherInFight() { other = other };
			base.Initialize(eff, subeffIndex);
		}
	}
}