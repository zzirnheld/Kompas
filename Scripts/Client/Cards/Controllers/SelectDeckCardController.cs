
using Godot;
using Kompas.Cards.Models;
using Kompas.Client.Cards.Views;
using Kompas.Client.UI.GameStart;

namespace Kompas.Cards.Controllers
{
	/// <summary>
    /// Extends TextureRect because the TextureRect has to be the parent, otherwise the grid doesn't line up.
    /// For some reason.
    /// </summary>
	public partial class SelectDeckCardController : Control
	{
		[Export]
		public SelectDeckInfoDisplayer InfoDisplayer { get; private set; }

		private CardBase card;
		public CardBase Card
		{
			get => card;
			protected set
			{
				card = value;
				myView.Show(card);
			}
		}

		private SelectDeckCardView myView;

		public override void _Ready() => myView = new SelectDeckCardView(InfoDisplayer);

		public void Init(CardBase card)
		{
			Card = card;
		}
	}
}