
using System;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;
using Kompas.Client.Cards.Views;
using Kompas.Client.Gamestate;
using Kompas.Client.Gamestate.Locations.Controllers;
using Kompas.Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.Cards.Controllers
{
	public partial class ClientCardController : Node3D, ICardController
	{
		[Export]
		private Zoomable3DCardInfoDisplayer? _infoDisplayer;
		private Zoomable3DCardInfoDisplayer InfoDisplayer => _infoDisplayer
			?? throw new UnassignedReferenceException();

		[Export]
		private CardMouseController? _mouseController;
		private CardMouseController MouseController => _mouseController
			?? throw new UnassignedReferenceException();

		[Export]
		private AnimationPlayer? _animationPlayer;
		private AnimationPlayer AnimationPlayer => _animationPlayer
			?? throw new UnassignedReferenceException();

		Node3D ICardController.Node => this;
		IGameCardInfo ICardController.Card => Card;

		private const string FocusedAnimationName = "Rotate";
		private const string ResetAnimationName = "RESET";

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

		private ClientGameCard? _card;
		public ClientGameCard Card
		{
			get => _card ?? throw new System.NullReferenceException("Tried to get card of CardController when it was null");
			set
			{
				if (_card != null) throw new System.InvalidOperationException("Already initialized ClientCardController's card");
				_card = value
					?? throw new System.ArgumentNullException(nameof(value), "Card can't be null!");
				CardView = new (InfoDisplayer, value);
				AOEController = GameController.TargetingController.SpacesController.AddAOE();
				
				Card.LocationChanged += (_, _) => RefreshAOE();
				Card.AugmentsChanged += (_, _) => RefreshAugments();
			}
		}

		private ClientGameController GameController => Card.ClientGame.ClientGameController;

		public void Delete() => QueueFree();

		public override void _Ready()
		{
			base._Ready();
			MouseController.HoverBegin += (_, _) => Hover();
			MouseController.HoverEnd += (_, _) => Unhover();
			MouseController.LeftClick += (_, doubleClick) => Select(doubleClick);
			MouseController.RightClick += (_, _) => ShowEffectDialog();
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

		public void RefreshAOE() => AOEController.Display(Card.SpaceInAOE, true);

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
			foreach (var card in Card.Augments)
			{
				var node = card.CardController.Node
					?? throw new System.NullReferenceException("ClientCardController must have non-null nodes!");
				this.TransferChild(node);
				node.Visible = true;
				node.Scale = Vector3.One * 0.2f;
				var rotation = card.Card.ControllingPlayer.Index * Mathf.Pi;
				node.Rotation = new Vector3(0, rotation, 0);
				node.Position = Vector3.Up * 0.05f; //TODO better spread
			}
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
			if (value) AnimationPlayer.Play(FocusedAnimationName);
			else AnimationPlayer.Play(ResetAnimationName);
		}

		public void RefreshTargeting()
		{
			CardView.Refresh();
		}
	}
}