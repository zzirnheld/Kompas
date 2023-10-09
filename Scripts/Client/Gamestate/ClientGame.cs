using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Client.Cards.Loading;
using Kompas.Client.Cards.Models;
using Kompas.Client.Effects.Controllers;
using Kompas.Client.Effects.Models;
using Kompas.Client.Gamestate.Locations.Models;
using Kompas.Client.Gamestate.Players;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Shared;

namespace Kompas.Client.Gamestate
{
	public class ClientGame : IGame
	{
		//TODO consider making a GameCardRepository non-generic base class that we can call stuff on when instantiating cards? 
		public ClientCardRepository ClientCardRepository => ClientGameController.CardRepository;
		public CardRepository CardRepository => ClientCardRepository;

		public ClientBoard ClientBoard { get; private set; }
		public Board Board => ClientBoard;

		public ClientStackController StackController { get; }

		IStackController IGame.StackController => StackController;

		private readonly ClientPlayer[] clientPlayers;
		public IPlayer[] Players => clientPlayers;
		public ClientPlayer FriendlyPlayer => clientPlayers[0];

		public ClientGameController ClientGameController { get; private set; }
		public GameController GameController => ClientGameController;

		private readonly Dictionary<int, ClientGameCard> cardsByID = new();
		public IReadOnlyCollection<GameCard> Cards => cardsByID.Values;

		public bool GameOver { get; private set; }
		public int TurnPlayerIndex { get; set; }
		public int FirstTurnPlayer { get; set; } //TODO
		public int RoundCount { get; set; } = 1;
		public int TurnCount { get; set; } = 1;

		//public ClientSettings ClientSettings => uiController.clientUISettingsController.ClientSettings;
		private ClientSettings settings; //TODO consider moving this to its own controller that Game references?
		public Settings Settings => settings;

		//search
		public ClientSearch search;

		//TODO move all of this to the GameController? maybe it should stay here and/or be moved out to a ClientGameTargetingController or something. aggressively single-responsibility
		/*
		//targeting
		public int targetsWanted;
		private GameCard[] currentPotentialTargets;
		public GameCard[] CurrentPotentialTargets
		{
			get => currentPotentialTargets;
			private set
			{
				currentPotentialTargets = value;
				ShowValidCardTargets();
			}
		}

		private (int, int)[] currentPotentialSpaces;
		public (int, int)[] CurrentPotentialSpaces
		{
			get => currentPotentialSpaces;
			set
			{
				currentPotentialSpaces = value;
				if (value != null) uiController.boardUIController.ShowSpaceTargets(space => value.Contains(space));
				else uiController.boardUIController.ShowSpaceTargets(_ => false);
			}
		}*/

		public bool canZoom = false;

		//dirty card set
		private readonly HashSet<GameCard> dirtyCardList = new();

		private int leyload;
		public int Leyload
		{
			get => leyload;
			set
			{
				leyload = value;
				//TODO refresh leyload shown + "next turn pips" shown
				//uiController.Leyload = Leyload;
				//Refresh next turn pips shown.
				//foreach (var player in Players) player.Pips = player.Pips;
			}
		}

		private ClientGame(ClientGameController gameController)
		{
			this.ClientGameController = gameController;

			clientPlayers = new ClientPlayer[2];
		}

		public static ClientGame Create(ClientGameController gameController)
		{
			var ret = new ClientGame(gameController);

			ret.ClientBoard = new ClientBoard(gameController.BoardController);

			/*
			ret.clientPlayers[0] = ClientPlayer.Create(ret, 0, gameController.PlayerControllers[0]);
			ret.clientPlayers[1] = ClientPlayer.Create(ret, 1, gameController.PlayerControllers[1]);

			ret.clientPlayers[0].Enemy = ret.clientPlayers[1];
			ret.clientPlayers[1].Enemy = ret.clientPlayers[0];
			*/ //TODO uncomment once player controllers are set up

			return ret;
		}

		public void GameEnded(bool victory)
		{
			if (GameOver) return;

			GameOver = true;
			//uiController.escapeMenuUIController.Enable();
			//TODO display you win/lose
			//TODO display rematch/main menu options? disallow user from closing menu?
		}

		//TODO: this is not a MonoBehavior! wake up sheeple!
		//move to GameController
		/*
		private void Awake()
		{
			uiController.clientUISettingsController.LoadSettings();
			ApplySettings();
		}*/

		public void AddCard(ClientGameCard card)
		{
			if (card.ID != -1) cardsByID.Add(card.ID, card);
		}

		//TODO to gamecontroller
		/*
		public void MarkCardDirty(GameCard card) => dirtyCardList.Add(card);

		public void PutCardsBack()
		{
			///foreach (var c in dirtyCardList) c.CardController?.PutBack();
			dirtyCardList.Clear();
		}*/

		public void SetAvatar(int player, string json, int avatarID)
		{
			if (player >= 2) throw new ArgumentException("Can only handle 2-player games!", nameof(player));

			var owner = clientPlayers[player];
			var avatar = ClientCardRepository.InstantiateClientAvatar(json, owner, avatarID, this);
			avatar.KnownToEnemy = true;
			owner.Avatar = avatar;
			Space to = player == 0 ? Space.NearCorner : Space.FarCorner;
			avatar.Play(to, owner);

			//TODO move to game controller - after avatar is set, show avatar in deck select screen
			//if (player == 1) uiController.connectionUIController.deckAcceptedUIController.ShowEnemyAvatar(avatar.FileName);
		}

		public void Delete(GameCard card)
		{
			card.Remove();
			cardsByID.Remove(card.ID);
			card.CardController.Delete(); //TODO consider moving to GameController
		}

		//requesting
		public void SetFirstTurnPlayer(int playerIndex)
		{
			FirstTurnPlayer = TurnPlayerIndex = playerIndex;
			//TODO move to GameController:
			//uiController.ChangeTurn(playerIndex);
			//uiController.connectionUIController.Hide();
			RoundCount = 1;
			TurnCount = 1;
			canZoom = true;
			//TODO move to GameController:
			//force updating of pips to show correct messages.
			//there's probably a better way to do this.
			//foreach (var player in Players) player.Pips = player.Pips;
		}

		public void SetTurn(int index)
		{
			TurnPlayerIndex = index;
			foreach (var c in Cards) c.ResetForTurn(this.TurnPlayer());
			//TODO move to GameController:
			//uiController.ChangeTurn(TurnPlayerIndex);
			if (TurnPlayerIndex == FirstTurnPlayer) RoundCount++;
			TurnCount++;
			//TODO move to GameController:
			//foreach (var player in Players) player.Pips = player.Pips;
		}

		public GameCard LookupCardByID(int id) => cardsByID.ContainsKey(id) ? cardsByID[id] : null;

		//TODO move to GameController:
		/*
		public void ShowCardsByZoom(ZoomLevel zoomed)
		{
			//TODO make this better with a dirty list
			foreach (var c in Cards.Where(c => c != null && c.CardController.gameObject.activeSelf))
			{
				c.CardController.gameCardViewController.Refresh();
			}
		}

		/// <summary>
		/// Makes cards show again, in case information changed after the packet.
		/// </summary>
		public void Refresh()
		{
			ShowCardsByZoom(ClientCameraController.Main.ZoomLevel);
			uiController.cardInfoViewUIController.Refresh();
		}

		public void EffectActivated(ClientEffect eff)
		{
			uiController.SetCurrState($"{(eff.ControllingPlayer.Friendly ? "Friendly" : "Enemy")} {eff.Source.CardName} Effect Activated",
				eff.blurb);
		}*/

		public void StackEmptied()
		{
			//TODO move to GameController:
			//uiController.TargetMode = TargetMode.Free;
			//uiController.SetCurrState("Nothing Happening");
			foreach (var c in Cards) c.ResetForStack();
			//ShowNoTargets();
		}

		//TODO move to GameController:
		/*
		public void ApplySettings()
		{
			ClientCameraController.ZoomThreshold = ClientSettings.zoomThreshold;
			uiController.ApplySettings(ClientSettings);
			foreach (var card in Cards) card.CardController.gameCardViewController.Refresh();
		}

		#region targeting
		/// <summary>
		/// Sets up the client for the player to select targets
		/// </summary>
		public void SetPotentialTargets(int[] ids, IListRestriction listRestriction)
		{
			CurrentPotentialTargets = ids?.Select(i => LookupCardByID(i)).Where(c => c != null).ToArray();
			searchCtrl.StartSearch(CurrentPotentialTargets, listRestriction);
		}

		public void ClearPotentialTargets()
		{
			CurrentPotentialTargets = null;
			searchCtrl.ResetSearch();
		}

		/// <summary>
		/// Makes each card no longer show any highlight about its status as a target
		/// </summary>
		public void ShowNoTargets()
		{
			foreach (var card in Cards) card.CardController.gameCardViewController.Refresh();
		}

		/// <summary>
		/// Show valid target highlight for current potential targets
		/// </summary>
		public void ShowValidCardTargets()
		{
			if (CurrentPotentialTargets != null)
			{
				foreach (var card in CurrentPotentialTargets) card.CardController.gameCardViewController.Refresh();
			}
			else ShowNoTargets();
		}

		public override bool IsCurrentTarget(GameCard card) => searchCtrl.IsCurrentlyTargeted(card);
		public override bool IsValidTarget(GameCard card) => searchCtrl.IsValidTarget(card);

		public override CardBase FocusedCard => uiController.cardInfoViewUIController.FocusedCard;
		#endregion targeting
		*/
	}
}