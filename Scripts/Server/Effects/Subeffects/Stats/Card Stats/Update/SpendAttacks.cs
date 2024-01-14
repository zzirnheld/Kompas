using Kompas.Effects.Models.Identities.Numbers;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class SpendAttacks : UpdateCardStats
	{
		public int modifier = 1;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			attacksThisTurn ??= new Constant() { constant = modifier };
			base.Initialize(eff, subeffIndex);
		}
	}
}
