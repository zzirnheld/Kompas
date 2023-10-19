using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models
{
	public abstract class Hand : OwnedLocationModel
	{
		private readonly List<GameCard> hand = new();
		public override IEnumerable<GameCard> Cards => hand;

		public override Location Location => Location.Hand;

		private readonly HandController handController;

		public int HandSize => hand.Count;

		protected Hand(IPlayer owner, HandController handController) : base(owner)
		{
			this.handController = handController;
			handController.HandModel = this; //TODO: is there another, better way to initialize HandModel? without leaking this
		}

		public GameCard this[int index] => hand[index];

		public override int IndexOf(GameCard card) => hand.IndexOf(card);

		protected override void PerformAdd(GameCard card, int? index, IStackable stackableCause)
		{
			if (index.HasValue) hand.Insert(index.Value, card);
			else hand.Add(card);
			handController.Refresh();
		}

		public override void Remove(GameCard card)
		{
			if (!hand.Contains(card)) throw new CardNotHereException(Location, card,
				$"Hand of \n{string.Join(", ", hand.Select(c => c.CardName))}\n doesn't contain {card}, can't remove it!");

			hand.Remove(card);
			handController.Refresh();
		}
	}
}