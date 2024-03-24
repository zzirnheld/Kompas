using System.Buffers;
using System.Collections.Generic;
using Godot;
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
		public IPlayer Owner { get; }

		public abstract Location Location { get; }

		public abstract IEnumerable<GameCard> Cards { get; }

		public OwnedLocationModel(IPlayer owner)
		{
			Owner = owner;
		}

		public bool IsLocation(Location location, bool friendly)
			=> location == Location
			&& friendly == Owner.Friendly;

		public abstract int IndexOf(GameCard card);

		public abstract void Remove(GameCard card);

		protected virtual bool AllowAlreadyHereWhenAdd => false;

		/// <summary>
		/// Adds the card to this owned game location at the relevant index.
		/// DOES NOT set the controller (that will need to be done manually by the implementer)
		/// </summary>
		public void Add(GameCard card, int? index = null, IStackable? stackableCause = null)
		{
			GD.Print($"Trying to {Location} {card}");
			if (card == null) throw new NullCardException($"Cannot add null card to {Location}");
			if (!AllowAlreadyHereWhenAdd && this == card.LocationModel) throw new AlreadyHereException(Location);

			//Check if the card is successfully removed (if it's not, it's probably an avatar)
			//TODO replace these with an AvatarRemovedException that gets caught
			try { card.Remove(stackableCause); }
			catch (AvatarRetreatedException)
			{
				GD.PushWarning($"{card}, an Avatar, retreated");
				return;
			}

			GD.Print($"{card} successfully removed, moving");
			PerformAdd(card, index, stackableCause);
		}

		/// <summary>
        /// Perform the operations that will place <see cref="card"/> at this location.
        /// Can be optionally overridden to add logic after validation, but before/after the add occurs
        /// </summary>
		protected virtual void PerformAdd(GameCard card, int? index, IStackable? stackableCause)
		{
			card.LocationModel = this;
			card.Position = null;
			card.ControllingPlayer = Owner;
			AddToCollection(card, index);
		}

		protected abstract void AddToCollection(GameCard card, int? index);
	}
}