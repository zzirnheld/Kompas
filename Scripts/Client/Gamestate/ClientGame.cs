
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Client.Cards.Loading;
using Kompas.Client.Cards.Models;
using Kompas.Client.Effects.Controllers;
using Kompas.Client.Effects.Models;
using Kompas.Client.Gamestate.Locations.Models;
using Kompas.Client.Gamestate.Players;
using Kompas.Client.Networking;
using Kompas.Client.UI;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Shared;
using Kompas.UI;

namespace Kompas.Client.Gamestate
{
	public class ClientGame : Game
	{
		private readonly ClientCardRepository cardRepository;
		public override CardRepository CardRepository => cardRepository;

		//Stored for lazy loading of ClientNetworkController - avoid leaking this
		private readonly TcpClient tcpClient;
		private ClientNetworkController _clientNetworkController;
		protected ClientNetworkController ClientNetworkController
			=> _clientNetworkController ??= new ClientNetworkController(tcpClient, this);
		private ClientNotifier _clientNotifier;
		public ClientNotifier ClientNotifier
			=> _clientNotifier ??= new ClientNotifier(ClientNetworkController);

		private readonly ClientBoard board;
		public override Board Board => board;

		public ClientStackController StackController { get; }

		private readonly ClientPlayer[] clientPlayers;
		public override Player[] Players => clientPlayers;
		public ClientPlayer FriendlyPlayer => clientPlayers[0];

		private readonly ClientUIController uiController;
		public override GameUIController UIController => uiController;

		private readonly Dictionary<int, ClientGameCard> cardsByID = new();
		public override IReadOnlyCollection<GameCard> Cards => cardsByID.Values;

		//public ClientSettings ClientSettings => uiController.clientUISettingsController.ClientSettings;
		private ClientSettings settings; //TODO consider moving this to its own controller that Game references?
		public override Settings Settings => settings;

		//turn players?
		public bool FriendlyTurn => TurnPlayer == FriendlyPlayer;

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

		public override bool NothingHappening => !StackController.StackEntries.Any();
		public override IEnumerable<IStackable> StackEntries => StackController.StackEntries;

		public bool canZoom = false;

		//dirty card set
		private readonly HashSet<GameCard> dirtyCardList = new();

		public override int Leyload
		{
			get => base.Leyload;
			set
			{
				base.Leyload = value;
				//TODO refresh leyload shown + "next turn pips" shown
				//uiController.Leyload = Leyload;
				//Refresh next turn pips shown.
				//foreach (var player in Players) player.Pips = player.Pips;
			}
		}

		public ClientGame(TcpClient tcpClient, ClientUIController uiController)
		{
			this.tcpClient = tcpClient;
			this.uiController = uiController;
		}

		public bool GameOver { get; private set; }

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
			var avatar = cardRepository.InstantiateClientAvatar(json, owner, avatarID, this);
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
			card.CardController.QueueFree(); //TODO consider moving to GameController
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
			ResetCardsForTurn();
			//TODO move to GameController:
			//uiController.ChangeTurn(TurnPlayerIndex);
			if (TurnPlayerIndex == FirstTurnPlayer) RoundCount++;
			TurnCount++;
			//TODO move to GameController:
			//foreach (var player in Players) player.Pips = player.Pips;
		}

		public override GameCard GetCardWithID(int id) => cardsByID.ContainsKey(id) ? cardsByID[id] : null;

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
			CurrentPotentialTargets = ids?.Select(i => GetCardWithID(i)).Where(c => c != null).ToArray();
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