using Kompas.Cards.Models;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Gamestate.Players.Server
{
	public class ServerPlayer : IPlayer
	{
		public IGame Game => throw new System.NotImplementedException();

		public IPlayer Enemy => throw new System.NotImplementedException();

		public int Pips { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
		public GameCard Avatar { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		public bool Friendly => throw new System.NotImplementedException();

		public int Index => throw new System.NotImplementedException();

		public Deck Deck => throw new System.NotImplementedException();

		public Discard Discard => throw new System.NotImplementedException();

		public Hand Hand => throw new System.NotImplementedException();

		public Annihilation Annihilation => throw new System.NotImplementedException();
	}
}