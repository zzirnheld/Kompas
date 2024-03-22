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

	public interface IAnnihilation<TCard, TPlayer> : ILocationModel<TCard, TPlayer>, IAnnihilation
		where TCard : class, IGameCard<TCard, TPlayer>
		where TPlayer : IPlayer<TCard, TPlayer>
	{
		public void Add(TCard card, int? index = null, IStackable? stackableCause = null);
	}

	public abstract class Annihilation<TCard, TPlayer> : OwnedLocationModel<TCard, TPlayer>, IAnnihilation<TCard, TPlayer>
		where TCard : class, IGameCard<TCard, TPlayer>
		where TPlayer : IPlayer<TCard, TPlayer>
	{
		private readonly IList<TCard> cards = new List<TCard>();
		public override IEnumerable<TCard> Cards => cards;

		public override Location Location => Location.Annihilation;
		private readonly AnnihilationController annihilationController;

		protected Annihilation(TPlayer owner, AnnihilationController annihilationController) : base(owner)
		{
			this.annihilationController = annihilationController;
			annihilationController.AnnihilationModel = this;
		}

		protected override void PerformAdd(TCard card, int? index, IStackable? stackableCause)
		{
			if (index.HasValue) cards.Insert(index.Value, card);
			else cards.Add(card);
			annihilationController.Refresh();
		}

		public override void Remove(TCard card)
		{
			if (!cards.Contains(card))
				throw new CardNotHereException(Location.Annihilation, card, "Card was not in annihilation, couldn't be removed");

			cards.Remove(card);
			annihilationController.Refresh();
		}

		public override int IndexOf(TCard card) => cards.IndexOf(card);
	}
}