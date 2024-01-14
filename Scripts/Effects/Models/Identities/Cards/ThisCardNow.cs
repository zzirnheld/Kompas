using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Effects.Models.Identities.Cards
{
	public class ThisCardNow : ContextlessLeafCardIdentityBase
	{
		protected override IGameCardInfo AbstractItem => InitializationContext.source
			?? throw new IllDefinedException();
	}
}