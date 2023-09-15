using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Board : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
	{
		protected override IReadOnlyCollection<GameCardBase> AbstractItem
			=> InitializationContext.game.Board.Cards.ToArray();
	}
}