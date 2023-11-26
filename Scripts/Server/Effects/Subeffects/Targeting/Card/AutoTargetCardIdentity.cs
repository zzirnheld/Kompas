using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class AutoTargetCardIdentity : ServerSubeffect
	{
		#nullable disable
		[JsonProperty (Required = Required.Always)]
		public IIdentity<IGameCardInfo> subeffectCardIdentity;
		#nullable restore

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