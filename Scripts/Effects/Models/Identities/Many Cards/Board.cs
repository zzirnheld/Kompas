using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	/// <summary>
	/// IMPL NOTE: Does not include those cards' augments.
	/// </summary>
	public class Board : ContextlessLeafIdentityBase<IReadOnlyCollection<IGameCardInfo>>
	{
		protected override IReadOnlyCollection<IGameCardInfo> AbstractItem
			=> InitializationContext.game.Board.Cards.ToArray();
	}
}