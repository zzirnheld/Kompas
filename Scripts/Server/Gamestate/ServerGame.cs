using System.Collections.Generic;
using System.Threading.Tasks;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Loading;
using Kompas.Server.Cards.Models;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Gamestate.Players;
using Kompas.Shared;

namespace Kompas.Server.Gamestate
{
	public class ServerGame : IGame
	{
		public const int MinDeckSize = 49;
		public const int AvatarEBonus = 15;

		private readonly ServerCardRepository cardRepo;
		public CardRepository CardRepository => cardRepo;
		public readonly ServerStackController effectsController;
		public readonly ServerPlayer[] serverPlayers; //TODO these should be init'd from TcpClients passed into the GameController, then passed into here
		public ServerBoardController serverBoardController;
		public override BoardController BoardController => serverBoardController;

		//UI
		public ServerUIController ServerUIController { get; private set; }
		public override UIController UIController => ServerUIController;
		public override Settings Settings => new ServerSettings();

		//Dictionary of cards, and the forwardings to make that convenient
		private readonly Dictionary<int, ServerGameCard> cardsByID = new Dictionary<int, ServerGameCard>();
		public override IReadOnlyCollection<GameCard> Cards => cardsByID.Values;

		//Players
		public override Player[] Players => serverPlayers;
		public ServerPlayer TurnServerPlayer => serverPlayers[TurnPlayerIndex];
		private int cardCount = 0;
		private int currPlayerCount = 0; //current number of players. shouldn't exceed 2

		//Effects-related concepts. Probably TODO move this into EffectsController
		public ServerEffect CurrEffect { get; set; }
		public override IStackable CurrStackEntry => effectsController.CurrStackEntry;
		public override IEnumerable<IStackable> StackEntries => effectsController.StackEntries;
		public override bool NothingHappening => effectsController.NothingHappening;

		public bool GameHasStarted { get; private set; } = false;

		public ServerPlayer Winner { get; private set; }

		public override int TurnCount
		{
			get => base.TurnCount;
			protected set
			{
				Leyload += value - TurnCount;
				base.TurnCount = value;
			}
		}

		public override int Leyload
		{
			get => base.Leyload;
			set
			{
				base.Leyload = value;
				serverPlayers[0].notifier.NotifyLeyload(Leyload);
			}
		}

		//Locks so we don't run into multithreading problems, but iirc this could break async. look into
		//(esp. since async is single threaded, not multithreaded)
		private readonly object AddCardsLock = new object();
		private readonly object CheckAvatarsLock = new object();

		public void AddCard(ServerGameCard card) => cardsByID.Add(card.ID, card);

		public void Init(ServerUIController uiController, ServerCardRepository cardRepo)
		{
			ServerUIController = uiController;
			this.cardRepo = cardRepo;
		}

		#region players and game starting
		public int AddPlayer(TcpClient tcpClient)
		{
			GD.Print($"Adding player #{currPlayerCount}");
			if (currPlayerCount >= 2) return -1;

			Players[currPlayerCount].SetInfo(tcpClient, currPlayerCount);
			currPlayerCount++;

			//if at least two players, start the game startup process by getting avatars
			if (currPlayerCount >= 2)
			{
				foreach (ServerPlayer p in serverPlayers) GetDeckFrom(p);
			}

			return currPlayerCount;
		}

		private void GetDeckFrom(ServerPlayer player) => player.notifier.GetDecklist();

		//TODO for future logic like limited cards, etc.
		private bool ValidDeck(List<string> deck)
		{
			//first name should be that of the Avatar
			if (!cardRepo.CardNameIsCharacter(deck[0]))
			{
				GD.PrintErr($"{deck[0]} isn't a character, so it can't be the Avatar");
				return false;
			}
			if (ServerUIController.DebugMode)
			{
				GD.PushWarning("Debug mode enabled, always accepting a decklist");
				return true;
			}
			if (deck.Count < MinDeckSize)
			{
				GD.PrintErr($"Deck {deck} too small");
				return false;
			}

			return true;
		}

		private List<string> SanitizeDeck(string decklist)
		{
			return decklist.Split('\n')
				.Where(c => !string.IsNullOrWhiteSpace(c) && CardRepository.CardExists(c))
				.ToList();
		}

		public async Task SetDeck(ServerPlayer player, string decklist)
		{
			List<string> deck = SanitizeDeck(decklist);

			if (ValidDeck(deck)) player.notifier.DeckAccepted();
			else
			{
				GetDeckFrom(player);
				return;
			}

			AvatarServerGameCard avatar;
			lock (AddCardsLock)
			{
				//otherwise, set the avatar and rest of the deck
				avatar = cardRepo.InstantiateServerAvatar(deck[0], player, cardCount++) ??
					throw new System.ArgumentException($"Failed to load avatar for card {deck[0]}");
				deck.RemoveAt(0); //take out avatar before telling player to add the rest of the deck
			}

			foreach (string name in deck)
			{
				ServerGameCard card;
				lock (AddCardsLock)
				{
					card = cardRepo.InstantiateServerNonAvatar(name, player, cardCount);
					if (card == null) continue;
					cardCount++;
				}
				GD.Print($"Adding new card {card.CardName} with id {card.ID}");
				player.deckCtrl.ShuffleIn(card);
				player.notifier.NotifyCreateCard(card, wasKnown: false);
			}

			player.Avatar = avatar;
			avatar.Play(player.AvatarCorner, player, new GameStartStackable());
			lock (CheckAvatarsLock)
			{
				if (Players.Any(player => player.Avatar == null)) return;
			}
			await StartGame();
		}

		public async Task StartGame()
		{
			//set initial pips to 0
			GD.Print($"Starting game. IPlayer 0 avatar is null? {Players[0].Avatar == null}. IPlayer 1 is null? {Players[1].Avatar == null}.");
			Players[0].Pips = 0;
			Players[1].Pips = 0;

			//determine who goes first and tell the players
			FirstTurnPlayer = Random.value > 0.5f ? 0 : 1;
			TurnPlayerIndex = FirstTurnPlayer;

			foreach (var p in serverPlayers)
			{
				p.notifier.SetFirstTurnPlayer(FirstTurnPlayer);
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
				if (TurnPlayerIndex == FirstTurnPlayer) RoundCount++;
				TurnCount++;
			}

			TurnServerPlayer.notifier.NotifyYourTurn();
			ResetCardsForTurn();

			TurnPlayer.Pips += Leyload;
			if (notFirstTurn) Draw(TurnPlayer);

			//do hand size
			effectsController.PushToStack(new ServerHandSizeStackable(this, TurnServerPlayer), default(TriggeringEventContext));

			//trigger turn start effects
			var context = new TriggeringEventContext(game: this, player: TurnServerPlayer);
			effectsController.TriggerForCondition(Trigger.TurnStart, context);

			await effectsController.CheckForResponse();
		}


		public async Task SwitchTurn()
		{
			TurnPlayerIndex = 1 - TurnPlayerIndex;
			GD.Print($"Turn swapping to the turn of index {TurnPlayerIndex}");

			await TurnStartOperations();
		}
		#endregion turn

		public List<GameCard> DrawX(Player controller, int x, IStackable stackSrc = null)
		{
			List<GameCard> drawn = new List<GameCard>();
			int cardsDrawn;
			for (cardsDrawn = 0; cardsDrawn < x; cardsDrawn++)
			{
				var toDraw = controller.deckCtrl.Topdeck;
				if (toDraw == null) break;

				var eachDrawContext = new TriggeringEventContext(game: this, CardBefore: toDraw, stackableCause: stackSrc, player: controller);
				toDraw.Hand(controller, stackSrc);
				eachDrawContext.CacheCardInfoAfter();
				effectsController.TriggerForCondition(Trigger.EachDraw, eachDrawContext);

				drawn.Add(toDraw);
			}
			var context = new TriggeringEventContext(game: this, stackableCause: stackSrc, player: controller, x: cardsDrawn);
			effectsController.TriggerForCondition(Trigger.DrawX, context);
			return drawn;
		}
		public GameCard Draw(Player player, IStackable stackSrc = null)
			=> DrawX(player, 1, stackSrc).FirstOrDefault();

		/// <param name="manual">Whether a player instigated the attack without an effect.</param>
		/// <returns>The Attack object created by starting this attack</returns>
		public ServerAttack Attack(GameCard attacker, GameCard defender, ServerPlayer instigator, IStackable stackSrc, bool manual = false)
		{
			GD.Print($"{attacker.CardName} attacking {defender.CardName} at {defender.Position}");
			//push the attack to the stack, then check if any player wants to respond before resolving it
			var attack = new ServerAttack(this, instigator, attacker, defender);
			effectsController.PushToStack(attack, new TriggeringEventContext(game: this, stackableCause: stackSrc, stackableEvent: attack, player: instigator));
			//check for triggers related to the attack (if this were in the constructor, the triggers would go on the stack under the attack
			attack.Declare(stackSrc);
			if (manual) attacker.AttacksThisTurn++;
			return attack;
		}

		public override GameCard GetCardWithID(int id) => cardsByID.ContainsKey(id) ? cardsByID[id] : null;

		public ServerPlayer ServerControllerOf(GameCard card) => serverPlayers[card.ControllerIndex];

		public void DumpGameInfo()
		{
			GD.Print("BEGIN GAME INFO DUMP");
			GD.Print("Cards:");
			foreach (var c in Cards) GD.Print(c.ToString());

			GD.Print($"Cards on board:\n{BoardController}");

			GD.Print(effectsController.ToString());
		}

		public override bool IsCurrentTarget(GameCard card) => false;
		public override bool IsValidTarget(GameCard card) => false;

		public void Lose(ServerPlayer player)
		{
			Winner = player.enemy;
			Winner.notifier.NotifyWin();
		}
	}
	}
}