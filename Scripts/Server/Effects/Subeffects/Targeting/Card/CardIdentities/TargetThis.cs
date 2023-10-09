using Kompas.Effects.Models.Identities.Cards;

namespace Kompas.Server.Effects.Subeffects
{
	public class TargetThis : AutoTargetCardIdentity
	{
		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			subeffectCardIdentity = new ThisCardNow();
			base.Initialize(eff, subeffIndex);
		}
	}
}