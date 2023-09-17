using Godot;
using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.UI.CardInfoDisplayers.DeckBuilder;
using Kompas.UI.DeckBuilder;

namespace Kompas.Cards.Controllers
{
	public partial class DeckBuilderCardController : Control
	{
		[Export]
		public DeckBuilderInfoDisplayer InfoDisplayer { get; private set; }

		protected DeckBuilderDeckController DeckController { get; private set; }

		private DeckBuilderCard card;
		protected DeckBuilderCard Card
		{
			get => card;
			set
			{
				card = value;
				myView.Show(card);
			}
		}

		private DeckBuilderTopLeftCardView topLeftCardView;

		private DeckBuilderCardView myView;

		public override void _Ready() => myView = new DeckBuilderCardView(InfoDisplayer);

		public void Init(DeckBuilderCard card, DeckBuilderTopLeftCardView topLeftCardView, DeckBuilderDeckController deckController)
		{
			DeckController = deckController;

			this.topLeftCardView = topLeftCardView;

			Card = card;
		}


		public void ShowInTopLeft() => topLeftCardView.Show(Card);
	}
}