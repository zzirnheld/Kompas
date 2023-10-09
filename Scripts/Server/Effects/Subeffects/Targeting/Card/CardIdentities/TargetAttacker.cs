using Kompas.Effects.Models.Identities.Cards;

namespace Kompas.Server.Effects.Subeffects
{
	public class TargetAttacker : AutoTargetCardIdentity
	{
		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			subeffectCardIdentity = new Attacker();
			base.Initialize(eff, subeffIndex);
		}
	}
}