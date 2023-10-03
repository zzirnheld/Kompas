using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class ThisCardNow : ContextlessLeafCardIdentityBase
	{
		protected override IGameCard AbstractItem => InitializationContext.source;
	}
}