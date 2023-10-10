using Kompas.Cards.Models;
using Kompas.Client.Gamestate.Locations.Models;
using Kompas.Client.Networking;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Networking;

namespace Kompas.Client.Gamestate.Players
{
	public class ClientPlayer : IPlayer
	{
		public IPlayer Enemy { get; set; }

		//TODO: this is a delegate/lazy getter because the Networker isn't initialized until after the game is created on the client.
		//Reorganize to be like the server
		public delegate Networker GetNetworker();
		private readonly GetNetworker getNetworker;
		public Networker Networker => getNetworker();

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
		private ClientPlayer(ClientGame game, int index, GetNetworker getNetworker)
		{
			this.game = game;
			Index = index;
			this.getNetworker = getNetworker;
		}

		public static ClientPlayer Create(ClientGame game, int index, PlayerController playerController, GetNetworker getNetworker)
		{
			var ret = new ClientPlayer(game, index, getNetworker);

			ret.Deck = new ClientDeck(ret, playerController.DeckController);
			ret.Hand = new ClientHand(ret, playerController.HandController);
			ret.Discard = new ClientDiscard(ret, playerController.DiscardController);
			ret.Annihilation = new ClientAnnihilation(ret, playerController.AnnihilationController);

			return ret;
		}
	}
}