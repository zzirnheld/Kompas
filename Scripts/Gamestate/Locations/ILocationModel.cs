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
	}

	public interface ILocationModel<CardType, PlayerType> : ILocationModel
		where CardType : class, IGameCard<CardType, PlayerType>
		where PlayerType : IPlayer<CardType, PlayerType>
	{
		public new IEnumerable<CardType> Cards { get; }

		public int IndexOf(CardType card);

		public void Remove(CardType card);
	}
}