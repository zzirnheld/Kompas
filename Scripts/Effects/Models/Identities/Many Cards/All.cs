using System.Collections.Generic;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class All : ContextlessLeafIdentityBase<IReadOnlyCollection<IGameCardInfo>>
	{
		protected override IReadOnlyCollection<IGameCardInfo> AbstractItem => InitializationContext.game.Cards;
	}
}