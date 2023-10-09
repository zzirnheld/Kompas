using System.Collections.Generic;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Deck : ContextlessLeafIdentityBase<IReadOnlyCollection<IGameCard>>
	{
		[JsonProperty]
		public bool friendly = true;
		[JsonProperty]
		public bool enemy = false;

		protected override IReadOnlyCollection<IGameCard> AbstractItem
		{
			get
			{
				var cards = new List<IGameCard>();
				if (friendly) cards.AddRange(InitializationContext.Owner.Deck.Cards);
				if (enemy) cards.AddRange(InitializationContext.Owner.Enemy.Deck.Cards);
				return cards;
			}
		}
	}
}