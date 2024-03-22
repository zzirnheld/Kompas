using System;
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
using Kompas.Shared.Exceptions;
using KompasServer.Effects;

namespace Kompas.Server.Gamestate
{
	public class ServerGame : IGame<ServerGameCard>
	{
		public const int MinDeckSize = 49;
		public const int AvatarEBonus = 15;

		private readonly ServerCardRepository serverCardRepository;
		public CardRepository CardRepository => serverCardRepository;
		private ServerStackController? _stackController;
		public ServerStackController StackController => _stackController
			?? throw new UseFactoryException();
		IStackController IGame.StackController => StackController;

		private IBoard<ServerGameCard, ServerPlayer>? _board;
		public IBoard<ServerGameCard, ServerPlayer> Board => _board
			?? throw new UseFactoryException();
		IBoard IGame.Board => Board;

		private ServerAwaiter? _awaiter;
		public ServerAwaiter Awaiter => _awaiter
			?? throw new UseFactoryException();

		public bool DebugMode => true;


		//Dictionary of cards, and the forwardings to make that convenient
		private readonly Dictionary<int, ServerGameCard> cardsByID = new();
		public IReadOnlyCollection<ServerGameCard> Cards => cardsByID.Values;
		IReadOnlyCollection<IGameCard> IGame.Cards => Cards;

		//Players
		private ServerPlayer[]? _serverPlayers;
		private ServerPlayer[] ServerPlayers => _serverPlayers
			?? throw new NotInitializedException();
		public IPlayer[] Players => ServerPlayers;

		private IPlayer? _turnPlayer;
		public IPlayer TurnPlayer => _turnPlayer
			?? throw new NotInitializedException();
		private int cardCount = 0;

		public bool GameHasStarted { get; private set; } = false;

		public IPlayer? Winner { get; private set; }

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


		public event EventHandler<IPlayer>? TurnChanged;

		private ServerGame(ServerGameController gameController, ServerCardRepository cardRepo)
		{
			ServerGameController = gameController;
			serverCardRepository = cardRepo;
		}

		public static ServerGame Create (ServerGameController gameController, ServerCardRepository cardRepo)
		{
			ServerGame ret = new(gameController, cardRepo);

			ret._stackController = new ServerStackController(ret);
			ret._board = new ServerBoard(gameController.BoardController, ret);
			ret._awaiter = new ServerAwaiter(ret);

			return ret;
		}

		public void SetPlayers(ServerPlayer[] players)
		{
			if (players.Length != 2) throw new System.ArgumentException("Games support only exactly 2 players!", nameof(players));

			_serverPlayers = players;
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
			if (DebugMode)
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
			var avatarName = decklist.avatarName ?? throw new NullReferenceException();
			//otherwise, set the avatar and rest of the deck
			avatar = serverCardRepository.InstantiateServerCard(avatarName, this, player, cardCount++, isAvatar: true) ??
				throw new System.ArgumentException($"Failed to load avatar for card {decklist.avatarName}");
			string avatarJson = CardRepository.GetJsonFromName(avatarName) ?? throw new NullReferenceException();
			ServerNotifier.SetFriendlyAvatar(player, avatarJson, avatar.ID);
			cardsByID[avatar.ID] = avatar;

			foreach (string name in decklist.deck)
			{
				ServerGameCard card;
				card = serverCardRepository.InstantiateServerCard(name, this, player, cardCount);
				if (card == null) continue;
				cardCount++;
				GD.Print($"Adding new card {card.CardName} with id {card.ID}");
				player.Deck.ShuffleIn(card);
				ServerNotifier.NotifyCreateCard(player, card, wasKnown: false);
			}

			player.Avatar = avatar;
			avatar.Play(player.AvatarCorner, player, new GameStartStackable());
			ServerNotifier.DeckAccepted(player);

			try
			{
				if (Players.All(player => player.Avatar != null)) await StartGame();
			}
			catch (NotInitializedException) { }
		}

		public void AddCard(ServerGameCard card)
		{
			if (cardsByID.ContainsKey(card.ID))
				throw new System.InvalidOperationException($"Can't add card {card} #{card.ID} to the lookup because that's already {cardsByID[card.ID]}!");

			cardsByID[card.ID] = card;
		}

		public async Task StartGame()
		{
			//set initial pips to 0
			GD.Print($"Starting game. IPlayer 0 avatar is null? {Players[0].Avatar == null}. IPlayer 1 is null? {Players[1].Avatar == null}.");
			Players[0].Pips = 0;
			Players[1].Pips = 0;

			//determine who goes first and tell the players
			FirstTurnPlayer = new System.Random().NextDouble() > 0.5f ? 0 : 1;
			_turnPlayer = Players[FirstTurnPlayer];
			ServerNotifier.SetFirstTurnPlayer(TurnPlayer);

			foreach (var p in Players)
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
			StackController.PushToStack(new ServerHandSizeStackable(this, TurnPlayer), ServerPlayers[TurnPlayer.Index], default);

			TurnChanged?.Invoke(this, TurnPlayer);

			//trigger turn start effects
			var context = new TriggeringEventContext(game: this, player: TurnPlayer);
			StackController.TriggerForCondition(Trigger.TurnStart, context);

			await StackController.CheckForResponse();
		}
		
		protected void ResetCardsForTurn()
		{
			foreach (var c in Cards) c.ResetForTurn(TurnPlayer);
		}


		public async Task SwitchTurn()
		{
			_turnPlayer = TurnPlayer.Enemy;
			GD.Print($"Turn swapping to the turn of index {TurnPlayer.Index}");

			await TurnStartOperations();
		}
		#endregion turn

		public List<GameCard> DrawX(IPlayer controller, int x, IStackable? stackSrc = null)
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
				StackController.TriggerForCondition(Trigger.EachDraw, eachDrawContext);

				drawn.Add(toDraw);
			}
			var context = new TriggeringEventContext(game: this, stackableCause: stackSrc, player: controller, x: cardsDrawn);
			StackController.TriggerForCondition(Trigger.DrawX, context);
			return drawn;
		}
		public GameCard? Draw(IPlayer player, IStackable? stackSrc = null)
			=> DrawX(player, 1, stackSrc).FirstOrDefault();

		/// <param name="manual">Whether a player instigated the attack without an effect.</param>
		/// <returns>The Attack object created by starting this attack</returns>
		public ServerAttack Attack(GameCard attacker, GameCard defender, ServerPlayer instigator, IStackable? stackSrc, bool manual = false)
		{
			GD.Print($"{attacker.CardName} attacking {defender.CardName} at {defender.Position}");
			//push the attack to the stack, then check if any player wants to respond before resolving it
			var attack = new ServerAttack(this, instigator, attacker, defender);
			StackController.PushToStack(attack, instigator, new TriggeringEventContext(game: this, stackableCause: stackSrc, stackableEvent: attack, player: instigator));
			//check for triggers related to the attack (if this were in the constructor, the triggers would go on the stack under the attack
			attack.Declare(stackSrc);
			if (manual) attacker.AttacksThisTurn++;
			return attack;
		}

		public GameCard? LookupCardByID(int id) => cardsByID.ContainsKey(id) ? cardsByID[id] : null;

		public ServerPlayer ServerControllerOf(GameCard card) => ServerPlayers[card.ControllingPlayerIndex];

		public void DumpGameInfo()
		{
			GD.Print("BEGIN GAME INFO DUMP");
			GD.Print("Cards:");
			foreach (var c in Cards) GD.Print(c.ToString());

			GD.Print($"Cards on board:\n{Board}");

			GD.Print(StackController.ToString());
		}

		public void Lose(ServerPlayer player)
		{
			Winner = player.Enemy;
			ServerNotifier.NotifyWin(player);
		}
	}
}