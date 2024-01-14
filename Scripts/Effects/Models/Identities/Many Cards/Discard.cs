using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
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
				var owner = InitializationContext.Owner ?? throw new IllDefinedException();
				if (friendly) cards.AddRange(owner.Discard.Cards);
				if (enemy) cards.AddRange(owner.Enemy.Discard.Cards);
				return cards;
			}
		}
	}
}