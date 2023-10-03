using System.Collections.Generic;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Discard : ContextlessLeafIdentityBase<IReadOnlyCollection<IGameCard>>
	{
		[JsonProperty]
		public bool friendly = true;
		[JsonProperty]
		public bool enemy = true;

		protected override IReadOnlyCollection<IGameCard> AbstractItem
		{
			get
			{
				var cards = new List<IGameCard>();
				if (friendly) cards.AddRange(InitializationContext.Controller.Discard.Cards);
				if (enemy) cards.AddRange(InitializationContext.Controller.Enemy.Discard.Cards);
				return cards;
			}
		}
	}
}