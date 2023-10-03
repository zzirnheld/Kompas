using Kompas.Cards.Models;
using Kompas.Client.Gamestate.Locations.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Players
{
	public class ClientPlayer : IPlayer
	{
		public IPlayer Enemy { get; set; }

		//TODO reconsider whether I'll ever need to have an IPlayer be aware of the type of its Game
		private readonly ClientGame game;
		public IGame Game => game;

		public Deck Deck { get; private set; }
		public Hand Hand { get; private set; }
		public Discard Discard { get; private set; }
		public Annihilation Annihilation { get; private set; }

		public int Index { get; }
		public GameCard Avatar { get; set; }

		public bool Friendly => Index == 0;

		public int Pips { get; set; } //Replace auto-property with value or possibly a hit on the controller/model?

		/// <summary>
		/// Private constructor to enforce factory to initialize game locations without leaking this
		/// </summary>
		private ClientPlayer(ClientGame game, int index)
		{
			this.game = game;
			Index = index;
		}

		public static ClientPlayer Create(ClientGame game, int index)
		{
			var ret = new ClientPlayer(game, index);

			ret.Deck = new ClientDeck(ret);
			ret.Hand = new ClientHand(ret);
			ret.Discard = new ClientDiscard(ret);
			ret.Annihilation = new ClientAnnihilation(ret);

			return ret;
		}
	}
}