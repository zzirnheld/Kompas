using Godot;
using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.UI.CardInfoDisplayers.DeckBuilder;

namespace Kompas.Cards.Controllers
{
	public partial class DeckBuilderCardController : Control
	{
		[Export]
		public DeckBuilderInfoDisplayer InfoDisplayer { get; private set; }

		private DeckBuilderCardView myView;
		private DeckBuilderTopLeftCardView topLeftCardView;

		private DeckBuilderCard card;

		public void Init(DeckBuilderCard card, DeckBuilderTopLeftCardView topLeftCardView)
		{
			this.card = card;
			myView.Show(card);
			this.topLeftCardView = topLeftCardView;
		}

		public override void _Ready() => myView = new DeckBuilderCardView(InfoDisplayer);

		public void ShowInTopLeft() => topLeftCardView.Show(card);
	}
}