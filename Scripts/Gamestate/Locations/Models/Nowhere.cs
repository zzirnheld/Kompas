using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models
{
	public sealed class Nowhere<CardType, PlayerType> : ILocationModel<CardType, PlayerType>
		where CardType : class, IGameCard<CardType, PlayerType>
		where PlayerType : IPlayer<CardType, PlayerType>
	{
		private static readonly Nowhere<CardType, PlayerType> _singleton = new();
		public static Nowhere<CardType, PlayerType> Instance => _singleton;

		public Location Location => Location.Nowhere;

		public IEnumerable<CardType> Cards => Enumerable.Empty<CardType>();
		IEnumerable<IGameCard> ILocationModel.Cards => Cards;
		public int IndexOf(CardType card) => -1;

		public void Remove(CardType card) { }
	}
}