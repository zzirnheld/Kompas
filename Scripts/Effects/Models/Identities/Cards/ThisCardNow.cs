using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class ThisCardNow : ContextlessLeafCardIdentityBase
	{
		protected override GameCardBase AbstractItem => InitializationContext.source;
	}
}