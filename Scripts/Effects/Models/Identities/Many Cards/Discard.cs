using System.Collections.Generic;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Discard : ContextlessLeafIdentityBase<IReadOnlyCollection<IGameCardInfo>>
	{
		[JsonProperty]
		public bool friendly = true;
		[JsonProperty]
		public bool enemy = true;

		protected override IReadOnlyCollection<IGameCardInfo> AbstractItem
		{
			get
			{
				var cards = new List<IGameCardInfo>();
				if (friendly) cards.AddRange(InitializationContext.Owner.Discard.Cards);
				if (enemy) cards.AddRange(InitializationContext.Owner.Enemy.Discard.Cards);
				return cards;
			}
		}
	}
}