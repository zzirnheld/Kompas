using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models
{
	public abstract class Annihilation : OwnedLocationModel
	{
		private readonly List<GameCard> cards = new();
		public override IEnumerable<GameCard> Cards => cards;

		public override Location Location => Location.Annihilation;
		private readonly AnnihilationController annihilationController;

		protected Annihilation(IPlayer owner, AnnihilationController annihilationController) : base(owner)
		{
			this.annihilationController = annihilationController;
			annihilationController.AnnihilationModel = this;
		}

		protected override void PerformAdd(GameCard card, int? index, IStackable stackableCause)
		{
			if (index.HasValue) cards.Insert(index.Value, card);
			else cards.Add(card);
			annihilationController.Refresh();
		}

		public override void Remove(GameCard card)
		{
			if (!cards.Contains(card))
				throw new CardNotHereException(Location.Annihilation, card, "Card was not in annihilation, couldn't be removed");

			cards.Remove(card);
			annihilationController.Refresh();
		}

		public override int IndexOf(GameCard card) => cards.IndexOf(card);
	}
}