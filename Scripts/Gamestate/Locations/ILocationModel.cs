using System.Collections.Generic;
using Kompas.Cards.Models;

namespace Kompas.Gamestate.Locations
{
	public interface ILocationModel
	{
		public Game Game { get; }
		public Location Location { get; }

		public IEnumerable<GameCard> Cards { get; }

		public int IndexOf(GameCard card);

		public void Remove(GameCard card);
	}
}