using System.Collections.Generic;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class All : ContextlessLeafIdentityBase<IReadOnlyCollection<IGameCard>>
	{
		protected override IReadOnlyCollection<IGameCard> AbstractItem => InitializationContext.game.Cards;
	}
}