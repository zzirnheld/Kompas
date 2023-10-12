
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Views;
using Kompas.Client.Cards.Models;
using Kompas.Client.Cards.Views;
using Kompas.Gamestate.Locations;

namespace Kompas.Client.Cards.Controllers
{
	public partial class ClientCardController : Node, ICardController
	{
		[Export]
		private Zoomable3DCardInfoDisplayer InfoDisplayer { get; set; }

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

		public bool Revealed
		{
			set
			{
				throw new System.NotImplementedException();
			}
		}

		public void Delete() => QueueFree();

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

		/*

		public GameObject revealedImage;

		public ClientCardMouseController mouseController;

		public ClientGame ClientGame => ClientCard.ClientGame;
		public ClientUIController ClientUIController => ClientGame.clientUIController;

		protected override Transform BoardTransform => ClientUIController.boardUIController.spaceCueCubesParent;

		public override void SetPhysicalLocation(Location location)
		{
			base.SetPhysicalLocation(location);
			ClientUIController.cardInfoViewUIController.Refresh();
		}


		public bool Revealed
		{
			set => revealedImage.SetActive(value);
		}

		private void OnDestroy()
		{
			//GD.Print("Destroying a client card ctrl. Destroying this ctrl's mouse ctrl.");
			Destroy(mouseController);
		}*/
	}
}