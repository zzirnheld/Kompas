using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models
{
	public sealed class Nowhere<TCard, TPlayer> : ILocationModel<TCard, TPlayer>
		where TCard : class, IGameCard<TCard, TPlayer>
		where TPlayer : IPlayer<TCard, TPlayer>
	{
		private static readonly Nowhere<TCard, TPlayer> _singleton = new();
		public static Nowhere<TCard, TPlayer> Instance => _singleton;

		public Location Location => Location.Nowhere;

		public IEnumerable<TCard> Cards => Enumerable.Empty<TCard>();
		IEnumerable<IGameCard> ILocationModel.Cards => Cards;
		public int IndexOf(TCard card) => -1;

		public void Remove(TCard card) { }
	}
}