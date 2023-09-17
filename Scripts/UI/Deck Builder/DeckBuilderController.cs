using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Views;
using Kompas.UI.CardInfoDisplayers;

namespace Kompas.UI.DeckBuilder
{
	public partial class DeckBuilderController : Control
	{
		[Export]
		private ControlInfoDisplayer CardInfoDisplayer { get; set; }

		public DeckBuilderCardRepository CardRepository { get; } = new DeckBuilderCardRepository();
		public DeckBuilderCardView CardView { get; private set; }

		public override void _Ready()
		{
			//TODO
			//CardView = new DeckBuilderCardView(CardInfoDisplayer);
		}
	}
}