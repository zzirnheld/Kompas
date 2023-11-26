using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Hand : ContextlessLeafIdentityBase<IReadOnlyCollection<IGameCardInfo>>
	{
		[JsonProperty]
		public bool friendly = true;
		[JsonProperty]
		public bool enemy = false;

		protected override IReadOnlyCollection<IGameCardInfo> AbstractItem
		{
			get
			{
				var cards = new List<IGameCardInfo>();
				var owner = InitializationContext.Owner ?? throw new IllDefinedException();
				if (friendly) cards.AddRange(owner.Hand.Cards);
				if (enemy) cards.AddRange(owner.Enemy.Hand.Cards);
				return cards;
			}
		}
	}
}