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
	public abstract class OwnedLocationModel<CardType, PlayerType> : ILocationModel<CardType, PlayerType>
		where CardType : class, IGameCard<CardType, PlayerType>
		where PlayerType : IPlayer<CardType, PlayerType>
	{
		public PlayerType Owner { get; }

		public abstract Location Location { get; }

		public abstract IEnumerable<CardType> Cards { get; }
		IEnumerable<IGameCard> ILocationModel.Cards => Cards;

		public OwnedLocationModel(PlayerType owner)
		{
			Owner = owner;
		}

		public bool IsLocation(Location location, bool friendly)
			=> location == Location
			&& friendly == Owner.Friendly;


		public abstract int IndexOf(CardType card);

		public abstract void Remove(CardType card);

		protected virtual bool AllowAlreadyHereWhenAdd => false;

		protected abstract void PerformAdd(CardType card, int? index, IStackable? stackableCause);

		/// <summary>
		/// Adds the card to this owned game location at the relevant index.
		/// DOES NOT set the controller (that will need to be done manually by the implementer)
		/// </summary>
		public void Add(CardType card, int? index = null, IStackable? stackableCause = null)
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
			card.LocationModel = this;
			card.Position = null;
			TakeControlOf(card);
			PerformAdd(card, index, stackableCause);
		}

		public abstract void TakeControlOf(CardType card);
	}
}