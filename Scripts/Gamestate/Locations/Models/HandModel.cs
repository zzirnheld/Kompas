using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Gamestate.Locations.Models
{
	public abstract class HandModel : OwnedLocationModel
	{
		public HandController HandController { get; init; }

		public override Location Location => Location.Hand;
		public override IEnumerable<GameCard> Cards => hand;

		protected readonly List<GameCard> hand = new List<GameCard>();

		public int HandSize => hand.Count;
		public override int IndexOf(GameCard card) => hand.IndexOf(card);

		public virtual bool Hand(GameCard card, IStackable stackSrc = null)
		{
			SharedAddValidation(card);

			var successful = card.Remove(stackSrc);
			if (successful)
			{
				GD.Print($"Handing {card.CardName}");

				hand.Add(card);
				card.LocationModel = this;
				card.Position = null;
				card.ControllingPlayer = Owner; //TODO should this be before or after the prev line?

				HandController.Refresh();
			}
			return successful;
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