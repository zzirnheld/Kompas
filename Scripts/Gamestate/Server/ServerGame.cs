using System.Collections.Generic;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Shared;

namespace Kompas.Gamestate.Server
{
	public class ServerGame : IGame
	{
		public bool DebugMode => throw new System.NotImplementedException();

		

		public GameController GameController => throw new System.NotImplementedException();

		public Settings Settings => throw new System.NotImplementedException();

		public Board Board => throw new System.NotImplementedException();

		public IPlayer[] Players => throw new System.NotImplementedException();

		public int TurnPlayerIndex => throw new System.NotImplementedException();

		public int FirstTurnPlayer => throw new System.NotImplementedException();

		public CardRepository CardRepository => throw new System.NotImplementedException();

		public IReadOnlyCollection<GameCard> Cards => throw new System.NotImplementedException();

		public int RoundCount => throw new System.NotImplementedException();

		public int TurnCount => throw new System.NotImplementedException();

		public int Leyload { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public IStackable CurrStackEntry => throw new System.NotImplementedException();

		public IEnumerable<IStackable> StackEntries => throw new System.NotImplementedException();

		public bool NothingHappening => throw new System.NotImplementedException();

		public GameCard LookupCardByID(int id)
		{
			throw new System.NotImplementedException();
		}
	}
}