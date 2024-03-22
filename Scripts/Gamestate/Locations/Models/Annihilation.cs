using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models
{
	public interface IAnnihilation : ILocationModel
	{ }

	public interface IAnnihilation<CardType> : ILocationModel<CardType>, IAnnihilation
		where CardType : GameCard
	{ }

	public abstract class Annihilation<CardType, PlayerType> : OwnedLocationModel<CardType, PlayerType>, IAnnihilation<CardType>
		where CardType : GameCard
		where PlayerType : IPlayer
	{
		private readonly IList<CardType> cards = new List<CardType>();
		public override IEnumerable<CardType> Cards => cards;

		public override Location Location => Location.Annihilation;
		private readonly AnnihilationController annihilationController;

		protected Annihilation(PlayerType owner, AnnihilationController annihilationController) : base(owner)
		{
			this.annihilationController = annihilationController;
			annihilationController.AnnihilationModel = this;
		}

		protected override void PerformAdd(CardType card, int? index, IStackable? stackableCause)
		{
			if (index.HasValue) cards.Insert(index.Value, card);
			else cards.Add(card);
			annihilationController.Refresh();
		}

		public override void Remove(CardType card)
		{
			if (!cards.Contains(card))
				throw new CardNotHereException(Location.Annihilation, card, "Card was not in annihilation, couldn't be removed");

			cards.Remove(card);
			annihilationController.Refresh();
		}

		public override int IndexOf(CardType card) => cards.IndexOf(card);
	}
}