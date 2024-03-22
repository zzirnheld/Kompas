using System.Collections.Generic;
using Kompas.Cards.Models;

namespace Kompas.Gamestate.Locations
{
	public interface ILocationModel
	{
		public Location Location { get; }

		public IEnumerable<GameCard> Cards { get; }
	}

	public interface ILocationModel<CardType> : ILocationModel
		where CardType : GameCard
	{
		public new IEnumerable<CardType> Cards { get; }

		public int IndexOf(CardType card);

		public void Remove(CardType card);
	}
}