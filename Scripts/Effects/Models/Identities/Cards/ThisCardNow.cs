using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class ThisCardNow : ContextlessLeafCardIdentityBase
	{
		protected override IGameCardInfo AbstractItem => InitializationContext.source;
	}
}