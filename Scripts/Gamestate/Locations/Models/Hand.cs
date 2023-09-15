using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Gamestate.Locations.Models
{
	public abstract class Hand : OwnedLocationModel
	{
		public HandController HandController { get; init; }

		public override Location Location => Location.Hand;
		public override IEnumerable<GameCard> Cards => hand;

		protected readonly List<GameCard> hand = new List<GameCard>();

		public int HandSize => hand.Count;
		public override int IndexOf(GameCard card) => hand.IndexOf(card);

		protected override void Add(GameCard card, int? index)
		{
			if (index.HasValue) hand.Insert(index.Value, card);
			else hand.Add(card);
			HandController.Refresh();
		}

		public override void Remove(GameCard card)
		{
			if (!hand.Contains(card)) throw new CardNotHereException(Location, card,
				$"Hand of \n{string.Join(", ", hand.Select(c => c.CardName))}\n doesn't contain {card}, can't remove it!");

			hand.Remove(card);
			HandController.Refresh();
		}
	}
}