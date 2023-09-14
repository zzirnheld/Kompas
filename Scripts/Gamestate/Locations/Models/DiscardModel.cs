using System.Collections.Generic;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Gamestate.Locations.Models
{
	public abstract class DiscardModel : OwnedLocationModel
	{
		public DiscardController discardController;

		protected readonly IList<GameCard> discard = new List<GameCard>();

		public override Location Location => Location.Discard;
		public override IEnumerable<GameCard> Cards => discard;

		//adding/removing cards
		public virtual bool Discard(GameCard card, IStackable stackSrc = null)
		{
			SharedAddValidation(card);

			//Check if the card is successfully removed (if it's not, it's probably an avatar)
			bool successful = card.Remove(stackSrc);
			if (successful)
			{
				GD.Print($"Discarding {card}");
				discard.Add(card);
				card.ControllingPlayer = Owner;
				card.LocationModel = this;
				card.Position = null;
				discardController.Refresh();
			}
			else GD.PrintErr($"Failed to discard {card}");
			return successful;
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