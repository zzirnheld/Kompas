using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Board : ContextlessLeafIdentityBase<IReadOnlyCollection<IGameCard>>
	{
		protected override IReadOnlyCollection<IGameCard> AbstractItem
			=> InitializationContext.game.Board.Cards.ToArray();
	}
}