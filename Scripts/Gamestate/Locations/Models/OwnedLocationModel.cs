using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models
{
	/// <summary>
	/// Base class for ILocationModels owned by a player (from whom we can infer what game they're in).
	/// Must have an ordering to the list.
	/// </summary>
	public abstract class OwnedLocationModel : ILocationModel
	{
		public abstract Game Game { get; }

		public abstract Location Location { get; }

		public abstract IEnumerable<GameCard> Cards { get; }

		public abstract int IndexOf(GameCard card);

		public abstract void Remove(GameCard card);

		protected virtual bool AllowAlreadyHereWhenAdd => false;

		protected abstract void Add(GameCard card, int? index);
		protected abstract void TakeControl(GameCard card);

		/// <summary>
        /// Adds the card to this owned game location at the relevant index.
        /// DOES NOT set the controller (that will need to be done manually by the implementer)
        /// </summary>
		protected void Add(GameCard card, int? index = null, IStackable stackableCause = null)
		{
			if (card == null) throw new NullCardException($"Cannot add null card to {Location}");
			if (!AllowAlreadyHereWhenAdd && this == card.LocationModel) throw new AlreadyHereException(Location);

			//Check if the card is successfully removed (if it's not, it's probably an avatar)
			//TODO replace these with an AvatarRemovedException that gets caught
			try { card.Remove(stackableCause); }
			catch (AvatarRetreatedException) { return; }

			card.LocationModel = this;
			card.Position = null;
			Add(card, index);
		}
	}
}