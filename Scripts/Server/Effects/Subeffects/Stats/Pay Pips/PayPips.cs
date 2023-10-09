using System.Threading.Tasks;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Numbers;

namespace Kompas.Server.Effects.Subeffects
{
	public class PayPips : ServerSubeffect
	{
		public override bool IsImpossible(TargetingContext targetingContext = null)
			=> GetPlayerTarget(targetingContext).Pips < ToPay;

		private int ToPay => pipCost.From(ResolutionContext, default);

		public IIdentity<int> pipCost = new EffectX();

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			pipCost.Initialize(DefaultInitializationContext);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			int toPay = ToPay;
			if (PlayerTarget.Pips < toPay) return Task.FromResult(ResolutionInfo.Impossible(CantAffordPips));

			PlayerTarget.Pips -= toPay;
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}