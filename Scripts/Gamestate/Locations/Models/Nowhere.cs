using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;

namespace Kompas.Gamestate.Locations.Models
{
	public sealed class Nowhere : ILocationModel
	{
		private static readonly Nowhere _singleton = new();
		public static Nowhere Instance => _singleton;

		public Location Location => Location.Nowhere;

		public IEnumerable<GameCard> Cards => Enumerable.Empty<GameCard>();

		public int IndexOf(GameCard card) => -1;

		public void Remove(GameCard card) { }
	}
}