using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Gamestate.Locations.Models
{
	public abstract class Discard : OwnedLocationModel
	{
		public DiscardController DiscardController { get; init; }

		protected readonly IList<GameCard> discard = new List<GameCard>();

		public override Location Location => Location.Discard;
		public override IEnumerable<GameCard> Cards => discard;

		protected override void Add(GameCard card, int? index)
		{
			if (index.HasValue) discard.Insert(index.Value, card);
			else discard.Add(card);
			DiscardController.Refresh();
		}

		public override void Remove(GameCard card)
		{
			if (!discard.Contains(card)) throw new CardNotHereException(Location.Discard, card);

			discard.Remove(card);
			DiscardController.Refresh();
		}

		public override int IndexOf(GameCard card) => discard.IndexOf(card);
	}
}