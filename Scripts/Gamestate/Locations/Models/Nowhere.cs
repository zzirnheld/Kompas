using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;

namespace Kompas.Gamestate.Locations.Models
{
	public sealed class Nowhere<CardType> : ILocationModel<CardType>
		where CardType : class, IGameCard<CardType>
	{
		private static readonly Nowhere<CardType> _singleton = new();
		public static Nowhere<CardType> Instance => _singleton;

		public Location Location => Location.Nowhere;

		public IEnumerable<CardType> Cards => Enumerable.Empty<CardType>();
		IEnumerable<IGameCard> ILocationModel.Cards => Cards;
		public int IndexOf(CardType card) => -1;

		public void Remove(CardType card) { }
	}
}