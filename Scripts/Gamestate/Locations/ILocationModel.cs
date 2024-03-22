using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations
{
	public interface ILocationModel
	{
		public Location Location { get; }

		public IEnumerable<IGameCard> Cards { get; }

		public bool IsLocation(Location location, bool friendly);
	}

	public interface ILocationModel<TCard, TPlayer> : ILocationModel
		where TCard : class, IGameCard<TCard, TPlayer>
		where TPlayer : IPlayer<TCard, TPlayer>
	{
		public new IEnumerable<TCard> Cards { get; }

		public int IndexOf(TCard card);

		public void Remove(TCard card);
	}
}