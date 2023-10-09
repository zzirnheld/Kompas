using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class AutoTargetCardIdentity : ServerSubeffect
	{
		public IIdentity<IGameCard> subeffectCardIdentity;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			subeffectCardIdentity.Initialize(initializationContext: DefaultInitializationContext);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			var card = subeffectCardIdentity.From(ResolutionContext, default);
			if (card == null) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

			Effect.AddTarget(card.Card);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}