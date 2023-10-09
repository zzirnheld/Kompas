using KompasCore.Cards;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Cards;

namespace Kompas.Server.Effects.Subeffects
{
	public class TargetOtherInFight : AutoTargetCardIdentity
	{
		public IIdentity<GameCardBase> other = new TargetIndex();

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			subeffectCardIdentity = new OtherInFight() { other = other };
			base.Initialize(eff, subeffIndex);
		}
	}
}