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
using Kompas.Shared.Enumerable;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Gamestate;

public partial class ClientTargetingController : Node
{
	[Export]
	private ClientTopLeftCameraDisplayer? _topLeftInfoDisplayer;
	private ClientTopLeftCameraDisplayer TopLeftInfoDisplayer => _topLeftInfoDisplayer ?? throw new UnassignedReferenceException();
	[Export]
	private ReminderTextPopup? _reminderTextPopup;
	private ReminderTextPopup ReminderTextPopup => _reminderTextPopup ?? throw new UnassignedReferenceException();
	[Export]
	private ClientGameController? _gameController;
	private ClientGameController GameController => _gameController ?? throw new UnassignedReferenceException();
	[Export]
	private Control? _canDeclineFurtherTargetsButton;
	private Control CanDeclineFurtherTargetsButton => _canDeclineFurtherTargetsButton ?? throw new UnassignedReferenceException();
	[Export]
	private SpacesController? _spacesController;
	public SpacesController SpacesController => _spacesController ?? throw new UnassignedReferenceException();

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
	}

	private void StartSearch(ISearch newSearch)
	{
		currentSearch = newSearch;

		RefreshCardsForSearchChange();

		var locations = currentSearch.SearchedLocations;
		if (locations.Count == 1) GameController.Camera.GoTo(new ClientCameraController.LookingAt(locations.Single()));
		//TODO open the deck/discard for each one of these. should GameController or CameraController handle that?
	}

	private void EndSearch()
	{
		currentSearch = null;

		RefreshCardsForSearchChange();

		//When end search, go back to board (FUTURE: go back to what we were last looking at?)
		GameController.Camera.GoTo(new ClientCameraController.LookingAt((Location.Board, true)));
	}

	public bool CanDeclineFurtherTargets
	{
		set => CanDeclineFurtherTargetsButton.Visible = value;
	}

	public override void _Ready()
	{
		base._Ready();
		if (TopLeftInfoDisplayer == null) throw new System.NullReferenceException("Forgot to init");
		_topLeftCardView = new(ReminderTextPopup, TopLeftInfoDisplayer);
		TopLeftCardView.FocusChange += (_, change) =>
		{
			//TODO: maybe animate the currently shown card? that's probably more helpful? think about it
			//or maybe animate the currently hovered card slightly, like popping it up, but leave the selection as it is
			change.Old?.ClientCardController.ShowFocused(false);
			change.New?.ClientCardController.ShowFocused(true);

			// We need to refresh the can do highlights, but don't wanna waste time re-showing card information, so do this here to make sure they refresh when focus refreshes.
			// TODO: trigger this better when card location changes?
			// ideally would hook into card controller.LocationChange for focused card
			ShowCanDoHighlights(TopLeftCardView.ShownCard);
		};
		TopLeftCardView.CardShown += (_, change) =>
		{
			ShowCanDoHighlights(change.New);
		};
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
		if (currentSearch == null)
		{
			var notifier = SelectedCard?.ClientGame.ClientGameController.Notifier;
			if (SelectedCard?.Location == Location.Board) notifier?.RequestMove(SelectedCard, space.x, space.y);
			if (SelectedCard?.Location == Location.Hand) notifier?.RequestPlay(SelectedCard, space.x, space.y);
		}
		else currentSearch.Select(space);
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
		var notifier = LastSelectedCard?.ClientGame.ClientGameController.Notifier;
		if (currentSearch != null) currentSearch.Select(card);
		else if (LastSelectedCard?.Location == Location.Board
			 && card.Location == Location.Board)
			notifier?.RequestAttack(LastSelectedCard, card);
		else if (LastSelectedCard?.Location == Location.Hand
			 && card.Location == Location.Board
			 && LastSelectedCard?.CardType == 'A')
		{
			_ = card.Position ?? throw new NullSpaceOnBoardException(card);
			notifier?.RequestPlay(LastSelectedCard, card.Position.x, card.Position.y);
		}

		Select(card);
	}

	public void Highlight(ClientGameCard? card) => TopLeftCardView.Hover(card);

	public void Unhighlight(ClientGameCard? card)
	{
		if (ShownCard == card) TopLeftCardView.Hover(null);
	}

	public void StartCardSearch(IEnumerable<int> potentialTargetIDs, IListRestriction listRestriction, string targetBlurb)
	{
		var search = CardSearch.Create(potentialTargetIDs.Select(GameController.Game.LookupCardByID).NonNull(), listRestriction,
			GameController.Game, this, GameController.Notifier);

		if (search == null)
		{
			EndSearch();
			Logger.Err("Failed to initalize search.");
			return;
		}

		StartSearch(search);
		GameController.CurrentStateController.ShowCurrentStateInfo(targetBlurb);
		search.SearchFinished += (_, _) => EndSearch();
	}

	public void StartHandSizeSearch(IEnumerable<int> cardIDs, IListRestriction listRestriction)
	{
		var search = new HandSizeSearch(cardIDs.Select(GameController.Game.LookupCardByID).NonNull(), listRestriction,
			GameController.Game, this, GameController.Notifier);

		StartSearch(search);
		GameController.CurrentStateController.ShowCurrentStateInfo($"Reshuffle down to hand size");
		search.SearchFinished += (_, _) => EndSearch();
	}

	public void StartSpaceSearch(IEnumerable<Space> spaces, IEnumerable<Space> recommendedSpaces, string blurb)
	{
		var search = new SpaceSearch(spaces, recommendedSpaces, GameController.Notifier);

		StartSearch(search);
		GameController.CurrentStateController.ShowCurrentStateInfo(blurb);
		search.SearchFinished += (_, _) => EndSearch();
	}

	public void TargetAccepted() { }

	public void DeclineFurtherTargets()
	{
		_ = GameController ?? throw new System.NullReferenceException("Failed to initialize");
		EndSearch();
		GameController.Notifier.DeclineAnotherTarget();
	}

	public bool Searching() => currentSearch != null;
	public bool Searching(Location location, bool friendly) => currentSearch?.SearchedLocations.Contains((location, friendly)) ?? false;
	public bool IsValidTarget(GameCard card) => currentSearch?.IsValidTarget(card) ?? false;
	public bool IsSelectedTarget(GameCard card) => currentSearch?.IsCurrentTarget(card) ?? false;
	public bool IsUnselectedValidTarget(GameCard card) => IsValidTarget(card) && !IsSelectedTarget(card);

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