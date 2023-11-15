
using System;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;
using Kompas.Client.Cards.Views;
using Kompas.Client.Gamestate;

namespace Kompas.Client.Cards.Controllers
{
	public partial class ClientCardController : Node3D, ICardController
	{
		[Export]
		private Zoomable3DCardInfoDisplayer InfoDisplayer { get; set; }

		[Export]
		private CardMouseController MouseController { get; set; }

		[Export]
		private AnimationPlayer AnimationPlayer { get; set; }

		Node3D ICardController.Node => this;
		IGameCardInfo ICardController.Card => Card;

		private const string FocusedAnimationName = "Rotate";
		private const string ResetAnimationName = "RESET";
		private ClientGameController gameController;

		private ClientCardView _cardView;
		public ClientCardView CardView
		{
			get => _cardView;
			private set
			{
				if (_cardView != null) throw new System.InvalidOperationException("Already initialized ClientCardController's card view!");
				_cardView = value;
			}
		}

		private ClientGameCard _card;

		public event EventHandler<GameCard> AnythingRefreshed;
		public event EventHandler<GameCard> StatsRefreshed;
		public event EventHandler<GameCard> LinksRefreshed;
		public event EventHandler<GameCard> AugmentsRefreshed;
		public event EventHandler<GameCard> TargetingRefreshed;

		public ClientGameCard Card
		{
			get => _card;
			set
			{
				if (_card != null) throw new System.InvalidOperationException("Already initialized ClientCardController's card");
				_card = value;
				CardView = new (InfoDisplayer ?? throw new System.InvalidOperationException("You didn't populate the client card ctrl's info displayer"), Card);
				gameController = value.ClientGame.ClientGameController;
			}
		}

		public void Delete() => QueueFree();

		public override void _Ready()
		{
			base._Ready();
			MouseController.MouseOver += (_, _) => ShowInTopLeft();
			MouseController.LeftClick += (_, _) => FocusInTopLeft();
			MouseController.RightClick += (_, _) => ShowEffectDialog();
		}

		public void ShowInTopLeft() => gameController.TargetingController.Highlight(Card);

		public void FocusInTopLeft() => gameController.TargetingController.Select(Card);

		public void ShowEffectDialog() => gameController.UseEffectDialog.Display(this);

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
				var node = card.CardController.Node;
				node.GetParent()?.RemoveChild(node);
				AddChild(node);
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

		public void RefreshTargeting() => CardView.Refresh();
	}
}