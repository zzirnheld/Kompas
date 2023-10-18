
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;
using Kompas.Client.Cards.Views;
using Kompas.Gamestate.Locations;

namespace Kompas.Client.Cards.Controllers
{
	public partial class ClientCardController : Node3D, ICardController
	{
		[Export]
		private Zoomable3DCardInfoDisplayer InfoDisplayer { get; set; }

		[Export]
		private CardMouseController MouseController { get; set; }

		Node3D ICardController.Node => this;
		IGameCard ICardController.Card => Card;

		private ClientCardView _cardView;
		private ClientCardView CardView
		{
			get => CardView;
			set
			{
				if (_cardView != null) throw new System.InvalidOperationException("Already initialized ClientCardController's card view!");
				_cardView = value;
			}
		}

		private ClientGameCard _card;
		public ClientGameCard Card
		{
			get => _card;
			set
			{
				if (_card != null) throw new System.InvalidOperationException("Already initialized ClientCardController's card");
				_card = value;
				CardView = new (InfoDisplayer ?? throw new System.InvalidOperationException("You didn't populate the client card ctrl's info displayer"), Card);
			}
		}

		public void Delete() => QueueFree();

		public override void _Ready()
		{
			base._Ready();
			MouseController.MouseOver += (_, _) => ShowInTopLeft();
			MouseController.LeftClick += (_, _) => FocusInTopLeft();
		}

		public void ShowInTopLeft()
		{
			//TODO: initialize a GameController or some such that includes a reference to the top left view (once top left view exists)
			throw new System.NotImplementedException();
		}

		public void FocusInTopLeft()
		{
			throw new System.NotImplementedException();
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
			//Revealed = Card.KnownToEnemy && Card.InHiddenLocation && !Card.OwningPlayer.Friendly;
		}

		public void RefreshLinks()
		{
			throw new System.NotImplementedException();
		}

		public void RefreshStats()
		{
			throw new System.NotImplementedException();
		}

		public void SetPhysicalLocation(Location location)
		{
			throw new System.NotImplementedException();
		}
	}
}