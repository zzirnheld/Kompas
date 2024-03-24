using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models
{
	public abstract class Discard : OwnedLocationModel
	{
		private readonly DiscardController discardController;

		protected readonly List<GameCard> discard = new();

		public override Location Location => Location.Discard;
		public override IEnumerable<GameCard> Cards => discard;

		protected Discard(IPlayer owner, DiscardController discardController) : base(owner)
		{
			this.discardController = discardController;
			discardController.DiscardModel = this;
		}

		protected override void PerformAdd(GameCard card, int? index, IStackable? stackableCause)
		{
			base.PerformAdd(card, index, stackableCause);
			discardController.Refresh();
		}

		protected override void AddToCollection(GameCard card, int? index)
		{
			if (index.HasValue) discard.Insert(index.Value, card);
			else discard.Add(card);
		}

		public override void Remove(GameCard card)
		{
			if (!discard.Contains(card)) throw new CardNotHereException(Location.Discard, card);

			discard.Remove(card);
			discardController.Refresh();
		}

		public override int IndexOf(GameCard card) => discard.IndexOf(card);
	}
}