using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations
{
	/// <summary>
	/// Base class for ILocationModels owned by a player (from whom we can infer what game they're in)
	/// </summary>
	public abstract class OwnedLocationModel : ILocationModel
	{
		public abstract Player Owner { get; }
		public virtual Game Game => Owner.Game;

		public abstract Location Location { get; }

		public abstract IEnumerable<GameCard> Cards { get; }

		public abstract int IndexOf(GameCard card);

		public abstract void Remove(GameCard card);

		public override string ToString() => $"{GetType()} owned by {Owner}";

		protected void SharedAddValidation(GameCard card, bool allowAlreadyHere = false)
		{
			if (card == null) throw new NullCardException($"Cannot add null card to {Location}");
			if (!allowAlreadyHere && this == card.LocationModel) throw new AlreadyHereException(Location);
		}
	}
}