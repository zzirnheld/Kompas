
using Kompas.Cards.Controllers;
using Kompas.Client.Cards.Models;

namespace Kompas.Client.Cards.Controllers
{
	public partial class ClientCardController : CardController
	{
		private ClientGameCard card;
		public ClientGameCard ClientCard
		{
			get => card;
			set
			{
				if (card != null) throw new System.InvalidOperationException("Already initialized ClientCardController's card");
				card = value;
				//TODO make view do its thing
			}
		}

		public bool Revealed
		{
			set
			{
				throw new System.NotImplementedException();
			}
		}

		/*

		public GameObject revealedImage;

		public ClientCardMouseController mouseController;

		public ClientGameCard ClientCard { get; set; }
		public override GameCard Card => ClientCard;

		public ClientGame ClientGame => ClientCard.ClientGame;
		public ClientUIController ClientUIController => ClientGame.clientUIController;

		protected override Transform BoardTransform => ClientUIController.boardUIController.spaceCueCubesParent;

		public override void SetPhysicalLocation(CardLocation location)
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