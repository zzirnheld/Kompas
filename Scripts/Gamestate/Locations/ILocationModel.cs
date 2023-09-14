using System.Collections.Generic;

namespace Kompas.Gamestate.Locations
{
	public interface ILocationModel
	{
		public Game Game { get; }
		public Location Location { get; }

		public IEnumerable<GameCard> Cards { get; }

		public int IndexOf(GameCard card);

		public void Remove(GameCard card);

		public void Refresh();
	}
}