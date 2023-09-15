using System.Collections.Generic;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class All : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
	{
		protected override IReadOnlyCollection<GameCardBase> AbstractItem => InitializationContext.game.Cards;
	}
}