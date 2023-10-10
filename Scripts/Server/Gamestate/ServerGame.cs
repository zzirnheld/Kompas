using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Loading;
using Kompas.Server.Cards.Models;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate.Locations.Models;
using Kompas.Server.Gamestate.Players;
using Kompas.Server.Networking;
using Kompas.Shared;
using KompasServer.Effects;

namespace Kompas.Server.Gamestate
{
	public class ServerGame : IGame
	{
		public const int MinDeckSize = 49;
		public const int AvatarEBonus = 15;

		private readonly ServerCardRepository serverCardRepository;
		public CardRepository CardRepository => serverCardRepository;
		public readonly ServerStackController serverStackController;

		public Board Board { get; private set; }
		public ServerAwaiter Awaiter { get; private set; }

		public bool DebugMode => false;


		//Dictionary of cards, and the forwardings to make that convenient
		private readonly Dictionary<int, ServerGameCard> cardsByID = new();
		public IReadOnlyCollection<GameCard> Cards => cardsByID.Values;

		//Players
		private ServerPlayer[] ServerPlayers { get; set; }
		public IPlayer[] Players => ServerPlayers;
		public IPlayer TurnPlayer { get; private set; }
		private int cardCount = 0;

		public bool GameHasStarted { get; private set; } = false;

		public IPlayer Winner { get; private set; }

		private int _turnCount;
		public int TurnCount
		{
			get => _turnCount;
			protected set
			{
				Leyload += value - TurnCount;
				_turnCount = value;
			}
		}

		private int _leyload;
		public int Leyload
		{
			get => _leyload;
			set
			{
				_leyload = value;
				ServerNotifier.NotifyLeyload(Leyload, Players);
			}
		}

		private ServerGameController ServerGameController { get; init; }
		public GameController GameController => ServerGameController;

		public Settings Settings => throw new System.NotImplementedException();

		public int FirstTurnPlayer { get; private set; }
		public int RoundCount { get; private set; }

		public IStackController StackController => throw new System.NotImplementedException();

		public void AddCard(ServerGameCard card) => cardsByID.Add(card.ID, card);

		private ServerGame(ServerGameController gameController, ServerCardRepository cardRepo)
		{
			ServerGameController = gameController;
			this.serverCardRepository = cardRepo;
		}

		public static ServerGame Create (ServerGameController gameController, ServerCardRepository cardRepo)
		{
			ServerGame ret = new(gameController, cardRepo);

			ret.Board = new ServerBoard(gameController.BoardController, ret);
			ret.Awaiter = new ServerAwaiter(ret);

			return ret;
		}

		public void SetPlayers(ServerPlayer[] players)
		{
			if (players.Length != 2) throw new System.ArgumentException("Games support only exactly 2 players!", nameof(players));

			ServerPlayers = players;
			foreach (ServerPlayer p in ServerPlayers) GetDeckFrom(p);

		}

		#region players and game starting
		private void GetDeckFrom(ServerPlayer player) => ServerNotifier.GetDecklist(player);

		//TODO for future logic like limited cards, etc.
		private bool ValidDeck(List<string> deck)
		{
			//first name should be that of the Avatar
			if (!ServerCardRepository.CardNameIsCharacter(deck[0]))
			{
				GD.PrintErr($"{deck[0]} isn't a character, so it can't be the Avatar");
				return false;
			}
			/*
			if (ServerUIController.DebugMode)
			{
				GD.PushWarning("Debug mode enabled, always accepting a decklist");
				return true;
			}*/
			if (deck.Count < MinDeckSize)
			{
				GD.PrintErr($"Deck {deck} too small");
				return false;
			}

			return true;
		}

		public async Task SetDeck(ServerPlayer player, Decklist decklist)
		{
			//TODO sanitize

			if (ValidDeck(decklist.deck)) ServerNotifier.DeckAccepted(player);
			else
			{
				GetDeckFrom(player);
				return;
			}

			ServerGameCard avatar;
			//otherwise, set the avatar and rest of the deck
			avatar = serverCardRepository.InstantiateServerCard(decklist.avatarName, player, cardCount++) ??
				throw new System.ArgumentException($"Failed to load avatar for card {decklist.avatarName}");

			foreach (string name in decklist.deck)
			{
				ServerGameCard card;
				card = serverCardRepository.InstantiateServerCard(name, player, cardCount);
				if (card == null) continue;
				cardCount++;
				GD.Print($"Adding new card {card.CardName} with id {card.ID}");
				player.Deck.ShuffleIn(card);
				ServerNotifier.NotifyCreateCard(player, card, wasKnown: false);
			}

			player.Avatar = avatar;
			avatar.Play(player.AvatarCorner, player, new GameStartStackable());
			if (Players.Any(player => player.Avatar == null)) return;
			await StartGame();
		}

		public async Task StartGame()
		{
			//set initial pips to 0
			GD.Print($"Starting game. IPlayer 0 avatar is null? {Players[0].Avatar == null}. IPlayer 1 is null? {Players[1].Avatar == null}.");
			Players[0].Pips = 0;
			Players[1].Pips = 0;

			//determine who goes first and tell the players
			FirstTurnPlayer = new System.Random().NextDouble() > 0.5f ? 0 : 1;
			TurnPlayer = Players[FirstTurnPlayer];
			ServerNotifier.SetFirstTurnPlayer(TurnPlayer);

			foreach (var p in ServerPlayers)
			{
				p.Avatar.SetN(0, stackSrc: null);
				p.Avatar.SetE(p.Avatar.E + AvatarEBonus, stackSrc: null);
				p.Avatar.SetW(0, stackSrc: null);
				DrawX(p, 5, stackSrc: null);
			}

			GameHasStarted = true;

			await TurnStartOperations(notFirstTurn: false);
		}
		#endregion

		#region turn
		public async Task TurnStartOperations(bool notFirstTurn = true)
		{
			if (notFirstTurn)
			{
				if (TurnPlayer.Index == FirstTurnPlayer) RoundCount++;
				TurnCount++;
			}

			ServerNotifier.NotifyYourTurn(TurnPlayer);
			ResetCardsForTurn();

			TurnPlayer.Pips += Leyload;
			if (notFirstTurn) Draw(TurnPlayer);

			//do hand size
			serverStackController.PushToStack(new ServerHandSizeStackable(this, TurnPlayer), ServerPlayers[TurnPlayer.Index], default);

			//trigger turn start effects
			var context = new TriggeringEventContext(game: this, player: TurnPlayer);
			serverStackController.TriggerForCondition(Trigger.TurnStart, context);

			await serverStackController.CheckForResponse();
		}
		
		protected void ResetCardsForTurn()
		{
			foreach (var c in Cards) c.ResetForTurn(TurnPlayer);
		}


		public async Task SwitchTurn()
		{
			TurnPlayer = TurnPlayer.Enemy;
			GD.Print($"Turn swapping to the turn of index {TurnPlayer.Index}");

			await TurnStartOperations();
		}
		#endregion turn

		public List<GameCard> DrawX(IPlayer controller, int x, IStackable stackSrc = null)
		{
			List<GameCard> drawn = new();
			int cardsDrawn;
			for (cardsDrawn = 0; cardsDrawn < x; cardsDrawn++)
			{
				var toDraw = controller.Deck.Topdeck;
				if (toDraw == null) break;

				var eachDrawContext = new TriggeringEventContext(game: this, CardBefore: toDraw, stackableCause: stackSrc, player: controller);
				toDraw.Hand(controller, stackSrc);
				eachDrawContext.CacheCardInfoAfter();
				serverStackController.TriggerForCondition(Trigger.EachDraw, eachDrawContext);

				drawn.Add(toDraw);
			}
			var context = new TriggeringEventContext(game: this, stackableCause: stackSrc, player: controller, x: cardsDrawn);
			serverStackController.TriggerForCondition(Trigger.DrawX, context);
			return drawn;
		}
		public GameCard Draw(IPlayer player, IStackable stackSrc = null)
			=> DrawX(player, 1, stackSrc).FirstOrDefault();

		/// <param name="manual">Whether a player instigated the attack without an effect.</param>
		/// <returns>The Attack object created by starting this attack</returns>
		public ServerAttack Attack(GameCard attacker, GameCard defender, ServerPlayer instigator, IStackable stackSrc, bool manual = false)
		{
			GD.Print($"{attacker.CardName} attacking {defender.CardName} at {defender.Position}");
			//push the attack to the stack, then check if any player wants to respond before resolving it
			var attack = new ServerAttack(this, instigator, attacker, defender);
			serverStackController.PushToStack(attack, instigator, new TriggeringEventContext(game: this, stackableCause: stackSrc, stackableEvent: attack, player: instigator));
			//check for triggers related to the attack (if this were in the constructor, the triggers would go on the stack under the attack
			attack.Declare(stackSrc);
			if (manual) attacker.AttacksThisTurn++;
			return attack;
		}

		public GameCard LookupCardByID(int id) => cardsByID.ContainsKey(id) ? cardsByID[id] : null;

		public ServerPlayer ServerControllerOf(GameCard card) => ServerPlayers[card.ControllerIndex];

		public void DumpGameInfo()
		{
			GD.Print("BEGIN GAME INFO DUMP");
			GD.Print("Cards:");
			foreach (var c in Cards) GD.Print(c.ToString());

			GD.Print($"Cards on board:\n{Board}");

			GD.Print(serverStackController.ToString());
		}

		public void Lose(ServerPlayer player)
		{
			Winner = player.Enemy;
			ServerNotifier.NotifyWin(player);
		}
	}
}