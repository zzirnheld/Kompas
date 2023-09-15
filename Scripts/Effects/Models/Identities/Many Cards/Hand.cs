using System.Collections.Generic;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Hand : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
	{
		[JsonProperty]
		public bool friendly = true;
		[JsonProperty]
		public bool enemy = false;

		protected override IReadOnlyCollection<GameCardBase> AbstractItem
		{
			get
			{
				var cards = new List<GameCardBase>();
				if (friendly) cards.AddRange(InitializationContext.Controller.hand.Cards);
				if (enemy) cards.AddRange(InitializationContext.Controller.Enemy.hand.Cards);
				return cards;
			}
		}
	}
}