using System.Collections.Generic;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Discard : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
	{
		[JsonProperty]
		public bool friendly = true;
		[JsonProperty]
		public bool enemy = true;

		protected override IReadOnlyCollection<GameCardBase> AbstractItem
		{
			get
			{
				var cards = new List<GameCardBase>();
				if (friendly) cards.AddRange(InitializationContext.Controller.Discard.Cards);
				if (enemy) cards.AddRange(InitializationContext.Controller.Enemy.Discard.Cards);
				return cards;
			}
		}
	}
}