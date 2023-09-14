using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Gamestate.Locations.Models
{
	//Not abstract because Client uses this base class
	public abstract class AnnihilationModel : OwnedLocationModel
	{
		public AnnihilationController annihilationController;

		private readonly List<GameCard> cards = new List<GameCard>();

		public override Location Location => Location.Annihilation;
		public override IEnumerable<GameCard> Cards => cards;

		/// <summary>
		/// Annihilates the card
		/// </summary>
		/// <param name="card">The card to add to this game location</param>
		/// <returns><see langword="true"/> if the add was completely successful.<br></br>
		/// <see langword="false"/> if the add failed in a way that isn't considered "impossible" (i.e. removing an avatar)</returns>
		public virtual bool Annihilate(GameCard card, IStackable stackSrc = null)
		{
			SharedAddValidation(card);

			//Check if the card is successfully removed (if it's not, it's probably an avatar)
			//TODO replace these with an AvatarRemovedException that gets caught
			if (card.Remove(stackSrc))
			{
				cards.Add(card);
				card.LocationModel = this;
				card.ControllingPlayer = Owner;
				card.Position = null;
				annihilationController.Refresh();
				return true;
			}
			return false;
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