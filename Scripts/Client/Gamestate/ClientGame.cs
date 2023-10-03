
using System.Collections.Generic;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Client.Effects.Controllers;
using Kompas.Client.Effects.Models;
using Kompas.Client.Gamestate.Locations.Models;
using Kompas.Client.Gamestate.Players;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;
using Kompas.Shared;

namespace Kompas.Client.Gamestate
{
	public class ClientGame : Game
	{
		private readonly ClientCardRepository cardRepo;
		public override CardRepository CardRepository => cardRepo;

		private readonly ClientNetworkController clientNetworkCtrl;
		private readonly ClientNotifier clientNotifier;

		private readonly ClientBoard board;
		public override Board Board => board;

		public ClientStackController StackController { get; }

		private readonly ClientPlayer[] clientPlayers;
		public override Player[] Players => clientPlayers;
		public ClientPlayer FriendlyPlayer => clientPlayers[0];

		private readonly ClientUIController clientUIController;
		public override UIController UIController => clientUIController;


		private readonly Dictionary<int, ClientGameCard> cardsByID = new Dictionary<int, ClientGameCard>();
		public override IReadOnlyCollection<GameCard> Cards => cardsByID.Values;

		public ClientSettings ClientSettings => clientUIController.clientUISettingsController.ClientSettings;
		public override Settings Settings => ClientSettings;

		//turn players?
		public bool FriendlyTurn => TurnPlayer == FriendlyPlayer;

		//search
		public ClientSearch search;

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
				if (value != null) clientUIController.boardUIController.ShowSpaceTargets(space => value.Contains(space));
				else clientUIController.boardUIController.ShowSpaceTargets(_ => false);
			}
		}

		public override bool NothingHappening => !stack.StackEntries.Any();
		public override IEnumerable<IStackable> StackEntries => stack.StackEntries;

		public bool canZoom = false;

		//dirty card set
		private readonly HashSet<GameCard> dirtyCardList = new HashSet<GameCard>();

		public override int Leyload
		{
			get => base.Leyload;
			set
			{
				base.Leyload = value;
				clientUIController.Leyload = Leyload;
				//Refresh next turn pips shown.
				foreach (var player in Players)
				{
					player.Pips = player.Pips;
				}
			}
		}

		public bool GameOver { get; private set; }

		public void GameEnded(bool victory)
		{
			if (GameOver) return;

			GameOver = true;
			clientUIController.escapeMenuUIController.Enable();
			//TODO display you win/lose
			//TODO display rematch/main menu options? disallow user from closing menu?
		}

		private void Awake()
		{
			clientUIController.clientUISettingsController.LoadSettings();
			ApplySettings();
		}

		public void AddCard(ClientGameCard card)
		{
			if (card.ID != -1) cardsByID.Add(card.ID, card);
		}

		public void MarkCardDirty(GameCard card) => dirtyCardList.Add(card);

		public void PutCardsBack()
		{
			foreach (var c in dirtyCardList) if (c.CardController != null) c.CardController.PutBack();
			dirtyCardList.Clear();
		}

		public void SetAvatar(int player, string json, int avatarID)
		{
			if (player >= 2) throw new System.ArgumentException();

			var owner = clientPlayers[player];
			var avatar = cardRepo.InstantiateClientAvatar(json, owner, avatarID);
			avatar.KnownToEnemy = true;
			owner.Avatar = avatar;
			Space to = player == 0 ? Space.NearCorner : Space.FarCorner;
			avatar.Play(to, owner);

			if (player == 1) clientUIController.connectionUIController.deckAcceptedUIController.ShowEnemyAvatar(avatar.FileName);
		}

		public void Delete(GameCard card)
		{
			card.Remove();
			cardsByID.Remove(card.ID);
			Destroy(card.CardController.gameObject);
		}

		//requesting
		public void SetFirstTurnPlayer(int playerIndex)
		{
			FirstTurnPlayer = TurnPlayerIndex = playerIndex;
			clientUIController.ChangeTurn(playerIndex);
			clientUIController.connectionUIController.Hide();
			RoundCount = 1;
			TurnCount = 1;
			canZoom = true;
			//force updating of pips to show correct messages.
			//there's probably a better way to do this.
			foreach (var player in Players) player.Pips = player.Pips;
		}

		public void SetTurn(int index)
		{
			TurnPlayerIndex = index;
			ResetCardsForTurn();
			clientUIController.ChangeTurn(TurnPlayerIndex);
			if (TurnPlayerIndex == FirstTurnPlayer) RoundCount++;
			TurnCount++;
			foreach (var player in Players) player.Pips = player.Pips;
		}

		public override GameCard GetCardWithID(int id) => cardsByID.ContainsKey(id) ? cardsByID[id] : null;

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
			clientUIController.cardInfoViewUIController.Refresh();
		}

		public void EffectActivated(ClientEffect eff)
		{
			clientUIController.SetCurrState($"{(eff.ControllingPlayer.Friendly ? "Friendly" : "Enemy")} {eff.Source.CardName} Effect Activated",
				eff.blurb);
		}

		public void StackEmptied()
		{
			clientUIController.TargetMode = TargetMode.Free;
			clientUIController.SetCurrState("Nothing Happening");
			foreach (var c in Cards) c.ResetForStack();
			ShowNoTargets();
		}

		public void ApplySettings()
		{
			ClientCameraController.ZoomThreshold = ClientSettings.zoomThreshold;
			clientUIController.ApplySettings(ClientSettings);
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

		public override CardBase FocusedCard => clientUIController.cardInfoViewUIController.FocusedCard;
		#endregion targeting
	}
}