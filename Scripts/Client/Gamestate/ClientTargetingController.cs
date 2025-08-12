using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Models;
using Kompas.Client.Cards.Models;
using Kompas.Client.Cards.Views;
using Kompas.Client.Gamestate.Locations.Controllers;
using Kompas.Client.Gamestate.Search;
using Kompas.Client.UI;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Server.Effects.Models.Subeffects;
using Kompas.Shared.Enumerable;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate;

public partial class ClientTargetingController : Node
{
	[Export]
	private ClientTopLeftCameraDisplayer? _topLeftInfoDisplayer;
	private ClientTopLeftCameraDisplayer TopLeftInfoDisplayer
		=> _topLeftInfoDisplayer ?? throw new UnassignedReferenceException(nameof(_topLeftInfoDisplayer), this);

	[Export]
	private ReminderTextPopup? _reminderTextPopup;
	private ReminderTextPopup ReminderTextPopup
		=> _reminderTextPopup ?? throw new UnassignedReferenceException(nameof(_reminderTextPopup), this);

	[Export]
	private ClientGameController? _gameController;
	private ClientGameController GameController
		=> _gameController ?? throw new UnassignedReferenceException(nameof(_gameController), this);

	[Export]
	private Control? _canDeclineFurtherTargetsButton;
	private Control CanDeclineFurtherTargetsButton
		=> _canDeclineFurtherTargetsButton ?? throw new UnassignedReferenceException(nameof(_canDeclineFurtherTargetsButton), this);

	[Export]
	private SpacesController? _spacesController;
	public SpacesController SpacesController
		=> _spacesController ?? throw new UnassignedReferenceException(nameof(_spacesController), this);

	[Export]
	private DeckController[]? _deckControllers;
	public DeckController[] DeckControllers => _deckControllers
		?? throw new UnassignedReferenceException(nameof(_deckControllers), this);

	private ClientTopLeftCameraView? _topLeftCardView;
	public ClientTopLeftCameraView TopLeftCardView => _topLeftCardView ?? throw new NotReadyYetException();

	/// <summary>
	/// The view already contains the logic for focusing on a given card, for entirely historical reasons.
	/// I could rip that out and move it here, but why bother
	/// </summary>
	public ClientGameCard? SelectedCard => TopLeftCardView.FocusedCard;
	public ClientGameCard? ShownCard => TopLeftCardView.ShownCard;

	private ClientGameCard? LastSelectedCard { get; set; }

	private ISearch? currentSearch;

	private void RefreshCardsForSearchChange()
	{
		foreach (var card in GameController.Game.Cards) card.CardController.RefreshTargeting();
		TopLeftCardView.Refresh();
		foreach (var ctrl in DeckControllers) ctrl.Refresh();
	}

	private void StartSearch(ISearch newSearch)
	{
		currentSearch = newSearch;

		RefreshCardsForSearchChange();

		var locations = currentSearch.SearchedLocations;
		if (locations.Count == 1)
		{
			GameController.Camera.GoTo(new ClientCameraController.LookingAt(locations.Single()), stash: true);
		}
	}

	private void EndSearch(object? _o, EventArgs _e) => EndSearch();
	private void EndSearch()
	{
		currentSearch = null;
		RefreshCardsForSearchChange();
		GameController.Camera.RestoreCurrentLook();
	}

	public bool CanDeclineFurtherTargets
	{
		set => CanDeclineFurtherTargetsButton.Visible = value;
	}

	public override void _Ready()
	{
		base._Ready();
		_topLeftCardView = new(TopLeftInfoDisplayer);
		TopLeftCardView.FocusChange += (_, change) =>
		{
			//TODO: maybe animate the currently shown card? that's probably more helpful? think about it
			//or maybe animate the currently hovered card slightly, like popping it up, but leave the selection as it is
			change.Old?.ClientCardController.ShowFocused(false);
			change.New?.ClientCardController.ShowFocused(true);
		};
		TopLeftCardView.CardShown += (_, change) =>
		{
			ShowCanDoHighlights(change.New);

			// We also want to make sure the highlights update if anything relevant changes.
			// TODO: maybe create an event that's fired only when something changes that could affect can do highlights?
			// that gets into the weeds of responsibilty, tho - who should know? because in theory I could create a card
			// that moves based on W or something, so that might include info not specified rn.
			// I think the smarter approach will be to evaluate whether the refresh changed the set of accessible things.
			// I think it's also just... not a super expensive calculation. If performance concerns arise, deal with this then.
			if (change.Old != null) change.Old.ClientCardController.AnythingRefreshed -= RefreshCanDoHighlights;
			if (change.New != null) change.New.ClientCardController.AnythingRefreshed += RefreshCanDoHighlights;
		};
	}

	private void RefreshCanDoHighlights(object? _, GameCard? card)
	{
		ShowCanDoHighlights(card);
	}

	/// <summary>
	/// <list type="bullet">
	/// 	<item>
	/// 		<term>If you didn't double-click</term>
	/// 		<description>We'd highlight space information. If that meant anything.</description>
	/// 	</item>
	/// 	<item>
	/// 		<term>If you did double click:</term>
	/// 		<description>If we're searching, select the <paramref name="space"/>.
	/// 			If not, try to play/move the currently selected card to the <paramref name="space"/>
	/// 		</description>
	/// 	</item>
	/// </list>
	/// </summary>
	public void Select(Space space, bool doubleClick)
	{
		if (!doubleClick) return;

		Logger.Log($"Selecting {space}");
		if (currentSearch != null)
		{
			currentSearch.Select(space);
			return;
		}

		if (SelectedCard is null) return;

		var notifier = SelectedCard.ClientGame.ClientGameController.Notifier;
		if (SelectedCard.Location == Location.Board) notifier.RequestMove(SelectedCard, space.x, space.y);
		if (SelectedCard.Location == Location.Hand) notifier.RequestPlay(SelectedCard, space.x, space.y);
	}

	/// <summary>
	/// Performs targeting actions for the selection of a particular card
	/// (as opposed to just hovering over it).
	/// </summary>
	public void Select(ClientGameCard? card)
	{
		Logger.Log($"Selecting {card}");
		LastSelectedCard = SelectedCard;

		TopLeftCardView.Focus(card);
		if (card == null) return;
	}

	/// <summary>
	/// Performs actions for the specific selection of the given <paramref name="card"/>,
	/// then selects it normally.
	/// <br/>
	/// Usually, this means that the <see cref="LastSelectedCard"/>
	/// will try to attack <paramref name="card"/>.
	/// </summary>
	public void SuperSelect(ClientGameCard card)
	{
		if (currentSearch != null) currentSearch.Select(card);
		else if (card.Location == Location.Board) SuperSelectOnBoard(card);

		Select(card);
	}

	private void SuperSelectOnBoard(ClientGameCard card)
	{
		if (LastSelectedCard?.Location is not Location lastSelectedLocation) return;

		var notifier = LastSelectedCard.ClientGame.ClientGameController.Notifier;

		switch (lastSelectedLocation)
		{
			case Location.Board:
				notifier.RequestAttack(LastSelectedCard, card);
				break;
			case Location.Hand:
				if (LastSelectedCard.CardType != 'A') break;
				var (x, y) = card.Position ?? throw new NullSpaceOnBoardException(card);
				notifier.RequestPlay(LastSelectedCard, x, y);
				break;
		}
	}

	public void Highlight(ClientGameCard? card) => TopLeftCardView.Hover(card);

	public void Unhighlight(ClientGameCard? card)
	{
		if (ShownCard == card) TopLeftCardView.Hover(null);
	}

	public void StartCardSearch(IEnumerable<int> potentialTargetIDs, IListRestriction listRestriction, IEnumerable<int> toSearchIDs, string targetBlurb)
	{
		var targets = potentialTargetIDs.Select(GameController.Game.LookupCardByID).NonNull();
		var search = CardSearch.Create(targets, listRestriction, toSearchIDs,
			GameController.Game, GameController.Notifier);

		if (search == null)
		{
			EndSearch();
			Logger.Err("Failed to initalize search.");
			return;
		}

		StartSearch(search);
		GameController.CurrentStateController.ShowCurrentStateInfo(targetBlurb);
		search.SearchFinished += EndSearch;
		search.HaveEnoughChanged += HaveEnough;
		HaveEnough(enough: search.HaveEnough);
	}

	private void HaveEnough(object? _ = null, bool enough = true) => CanDeclineFurtherTargets = enough;

	public void StartHandSizeSearch(IEnumerable<int> cardIDs, IListRestriction listRestriction)
	{
		var search = new HandSizeSearch(cardIDs.Select(GameController.Game.LookupCardByID).NonNull(), listRestriction,
			GameController.Game, GameController.Notifier);

		StartSearch(search);
		GameController.CurrentStateController.ShowCurrentStateInfo($"Reshuffle down to hand size");
		search.SearchFinished += EndSearch;
	}

	public void StartSpaceSearch(IEnumerable<Space> spaces, IEnumerable<Space> recommendedSpaces, string blurb)
	{
		var search = new SpaceSearch(spaces, recommendedSpaces, GameController.Notifier);

		StartSearch(search);
		GameController.CurrentStateController.ShowCurrentStateInfo(blurb);
		search.SearchFinished += EndSearch;
	}

	public void TargetAccepted() { }

	public void DeclineFurtherTargets()
	{
		if (currentSearch is not null && currentSearch.SendIfHaveEnough()) return;

		EndSearch();
		GameController.Notifier.DeclineAnotherTarget();
	}

	public bool Searching() => currentSearch != null;
	public bool Searching(Location location, bool friendly) => currentSearch?.SearchedLocations.Contains((location, friendly)) ?? false;
	public bool IsValidTarget(GameCard card) => currentSearch?.IsValidTarget(card) ?? false;
	public bool IsSelectedTarget(GameCard card) => currentSearch?.IsCurrentTarget(card) ?? false;
	public bool IsUnselectedValidTarget(GameCard card) => IsValidTarget(card) && !IsSelectedTarget(card);
	public bool IsBeingSearched(GameCard card) => currentSearch?.IsBeingSearched(card) ?? false;

	public void ShowCanDoHighlights(GameCard? card)
	{
		if (currentSearch != null)
		{
			SpacesController.DisplayCanTarget(currentSearch.IsRecommendedTarget, currentSearch.IsUnrecommendedTarget);
			return;
		}

		static bool recommendPlayTo(Space s, GameCard card)
			=> card.PlayRestriction.IsRecommendedNormalPlay((s, card.ControllingPlayer));
		//static bool canPlayTo(Space s, GameCard card)
		//	=> card.PlayRestriction.IsValid((s, card.ControllingPlayer), ResolutionContext.PlayerTrigger(null, card.Game));
		static bool canMoveTo(Space s, GameCard card)
			=> card.MovementRestriction.WouldBeValidNormalMoveInOpenGamestate(s);
		if (card == null) SpacesController.DisplayNone();
		else if (card.Location == Location.Board) SpacesController.DisplayCanMove(s => canMoveTo(s, card));
		else if (card.Location == Location.Hand) SpacesController.DisplayCanPlay(s => recommendPlayTo(s, card));
	}
}