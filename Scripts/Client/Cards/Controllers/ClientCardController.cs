using System;
using System.Linq;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Client.Cards.Models;
using Kompas.Client.Cards.Views;
using Kompas.Client.Gamestate;
using Kompas.Client.Gamestate.Locations.Controllers;
using Kompas.Client.UI;
using Kompas.Shared.Exceptions;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.Client.Cards.Controllers;

public partial class ClientCardController : Node3D, ICardController
{
	[Export]
	private CardModelController? _cardModelController;
	private CardModelController CardModelController => _cardModelController
		?? throw new UnassignedReferenceException(nameof(_cardModelController));

	[Export]
	private AnimationPlayer? _animationPlayer;
	public AnimationPlayer AnimationPlayer => _animationPlayer
		?? throw new UnassignedReferenceException();

	Node3D ICardController.Node => this;
	IGameCardInfo ICardController.Card => Card;

	private const string FocusedAnimationName = "Rotate";
	private const string ResetAnimationName = "RESET";
	private const string FlyUpAnimationName = "FlyUp";
	private const string FlyDownAnimationName = "FlyDown";

	public bool Focused { get; private set; }

	private ClientCardView? _cardView;
	public ClientCardView CardView
	{
		get => _cardView ?? throw new NotInitializedException();
		private set
		{
			if (_cardView != null) throw new System.InvalidOperationException("Already initialized ClientCardController's card view!");
			_cardView = value;
		}
	}

	private LinkedSpacesController? _aoeController;
	private LinkedSpacesController AOEController
	{
		get => _aoeController ?? throw new NotInitializedException();
		set
		{
			if (_aoeController != null) throw new System.InvalidOperationException("Already initialized ClientCardController's card aoe controller!");
			_aoeController = value;
		}
	}

	//TODO move these events to the card?
	public event EventHandler<GameCard?>? AnythingRefreshed;
	public event EventHandler<GameCard?>? StatsRefreshed;
	public event EventHandler<GameCard?>? LinksRefreshed;
	public event EventHandler<GameCard?>? AugmentsRefreshed;
	public event EventHandler<GameCard?>? TargetingRefreshed;
	public event EventHandler<GameCard?>? LocationRefreshed;

	private ClientGameCard? _card;
	public ClientGameCard Card
	{
		get => _card ?? throw new System.NullReferenceException("Tried to get card of CardController when it was null");
		set
		{
			if (_card != null) throw new System.InvalidOperationException("Already initialized ClientCardController's card");
			_card = value
				?? throw new System.ArgumentNullException(nameof(value), "Card can't be null!");
			CardView = new(CardModelController.InfoDisplayer, value);
			AOEController = GameController.TargetingController.SpacesController.AddAOE();
			//TODO: update AOE material accordingly, once that's something I have assigned

			Card.LocationChanged += (_, _) => RefreshLocation();
			Card.AugmentsChanged += (_, _) => RefreshAugments();
			Card.NegationChanged += (_, negated) => RefreshNegated(negated);
			Card.ActivationChanged += (_, activated) => RefreshActivated(activated);
		}
	}

	private ClientGameController GameController => Card.ClientGame.ClientGameController;

	public void Delete() => QueueFree();

	public override void _Ready()
	{
		base._Ready();
		CardModelController.MouseController.HoverBegin += (_, _) => Hover();
		CardModelController.MouseController.HoverEnd += (_, _) => Unhover();
		CardModelController.MouseController.LeftClick += (_, doubleClick) => Select(doubleClick);
		CardModelController.MouseController.RightClick += (_, _) => ShowEffectDialog();
	}

	public void Hover() => GameController.TargetingController.Highlight(Card);
	public void Unhover() => GameController.TargetingController.Unhighlight(Card);

	public void Select(bool superSelect)
	{
		if (superSelect) GameController.TargetingController.SuperSelect(Card);
		else GameController.TargetingController.Select(Card);
	}

	//TODO: right clicking an enemy should show a dialog that includes an attack button,
	//in case that's more natural for some people.
	public void ShowEffectDialog() => GameController.UseEffectDialog.Display(this);

	public void RefreshLocation()
	{
		AOEController.Display(Card.SpaceInAOE, true);

		LocationRefreshed?.Invoke(this, Card);
		AnythingRefreshed?.Invoke(this, Card);
	}

	public void RefreshNegated(bool negated)
	{
		CardModelController.HighlightsController.Negated = negated;
	}

	public void RefreshActivated(bool activated)
	{
		CardModelController.HighlightsController.Activated = activated;
	}

	/// <summary>
	/// TODO reimpl for godot
	/// Updates the model to show the little revealed eye iff the card:<br/>
	/// - is known to enemy<br/>
	/// - is in an otherwise hidden location<br/>
	/// - is controlled by an enemy<br/>
	/// </summary>
	public void RefreshRevealed()
	{
		CardView.Refresh();
		//Revealed = Card.KnownToEnemy && Card.InHiddenLocation && !Card.OwningPlayer.Friendly;
		AnythingRefreshed?.Invoke(this, Card);
	}

	public void RefreshLinks()
	{
		CardView.Refresh();
		//throw new System.NotImplementedException();
		AnythingRefreshed?.Invoke(this, Card);
		LinksRefreshed?.Invoke(this, Card);
	}

	public void RefreshAugments()
	{
		Card.AugmentedCard?.CardController.RefreshAugments();

		var cardControllers = Card.Augments.Select(c => c.CardController);
		if (Focused || cardControllers.Any(cc => cc.Focused)) CardModelController.AugmentsController.Spread(cardControllers);
		else CardModelController.AugmentsController.Stack(cardControllers);

		AnythingRefreshed?.Invoke(this, Card);
		AugmentsRefreshed?.Invoke(this, Card);
	}

	public void RefreshStats()
	{
		CardView.Refresh();
		AnythingRefreshed?.Invoke(this, Card);
		StatsRefreshed?.Invoke(this, Card);
	}

	public void ShowFocused(bool value)
	{
		Focused = value;
		if (value) AnimationPlayer.Play(name: FocusedAnimationName);
		else AnimationPlayer.Play(name: ResetAnimationName);
		RefreshAugments();
	}

	public void MoveToBoard(Action afterFlyUp)
	{
		Logger.Log($"{Card} moved to board!");
		if (!Visible || GetParent() == null)
		{
			//now that we remove from hand before adding to board, this is broken
			afterFlyUp();
			return;
		}

		AnimationPlayer.Play(name: FlyUpAnimationName);

		actualAnimationFinishedHandler = () =>
		{
			afterFlyUp();
			AnimationPlayer.Play(name: FlyDownAnimationName, customBlend: 0d);

			//Problem: after card flies down, it should resume animations as if focused on
			actualAnimationFinishedHandler = () => ShowFocused(GameController.TargetingController.TopLeftCardView.FocusedCard == Card);
			AnimationPlayer.AnimationFinished += AnimationFinishedHandler;
		};

		AnimationPlayer.AnimationFinished += AnimationFinishedHandler;
	}

	//TODO: I think this is the correct way to have the function be able to clean up itself.
	//This might create edge cases if I try and hang other stuff off this action.
	//probably worth thinking about
	private Action? actualAnimationFinishedHandler;

	private void AnimationFinishedHandler(StringName _)
	{
		AnimationPlayer.AnimationFinished -= AnimationFinishedHandler;
		actualAnimationFinishedHandler?.Invoke();
	}

	public void ShowEffectSource(bool current) => CardView.InfoDisplayer.DisplayEffectSource(current);

	public void RefreshTargeting()
	{
		CardView.Refresh();
	}

	public IHoverableCardInfoDisplayer PlaceCameraAboveCard(CameraFollowObject camera, uint movingCameraMask,
		uint arrivedCameraMask, Action<IHoverableCardInfoDisplayer> whenArrive)
	{
		//NOTE: this will not correctly account for clicking on a card, until I create the duplicate board and have it appear there, or figure out some other workaround
		//TODO: try having a script on the camera that has a Node3D it constantly sets its own position w/r/t every physics update?
		camera.Follow(CardModelController.CameraPosition, movingCameraMask, arrivedCameraMask, () => whenArrive(CardModelController.InfoDisplayer));
		return CardModelController.InfoDisplayer;
	}
}