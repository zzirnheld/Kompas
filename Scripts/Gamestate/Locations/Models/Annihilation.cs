using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Gamestate.Locations.Models
{
	public abstract class Annihilation : OwnedLocationModel
	{
		public AnnihilationController AnnihilationController { get; init; }

		private readonly List<GameCard> cards = new();

		public override Location Location => Location.Annihilation;
		public override IEnumerable<GameCard> Cards => cards;

		protected override void PerformAdd(GameCard card, int? index)
		{
			if (index.HasValue) cards.Insert(index.Value, card);
			else cards.Add(card);
			AnnihilationController.Refresh();
		}

		public override void Remove(GameCard card)
		{
			if (!cards.Contains(card))
				throw new CardNotHereException(Location.Annihilation, card, "Card was not in annihilation, couldn't be removed");

			cards.Remove(card);
			AnnihilationController.Refresh();
		}

		public override int IndexOf(GameCard card) => cards.IndexOf(card);
	}
}