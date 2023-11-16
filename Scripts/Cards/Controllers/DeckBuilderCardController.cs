using Godot;
using Kompas.Cards.Models;
using Kompas.Cards.Views;
using Kompas.UI.CardInfoDisplayers.DeckBuilder;
using Kompas.UI.DeckBuilder;

namespace Kompas.Cards.Controllers
{
	/// <summary>
    /// Extends TextureRect because the TextureRect has to be the parent, otherwise the grid doesn't line up.
    /// For some reason.
    /// </summary>
	public partial class DeckBuilderCardController : TextureRect
	{
		[Export]
		public DeckBuilderInfoDisplayer? InfoDisplayer { get; private set; }

		protected DeckBuilderDeckController? DeckController { get; private set; }

		private DeckBuilderCard? _card;
		public DeckBuilderCard? Card
		{
			get => _card;
			protected set
			{
				_card = value;
				_ = myView ?? throw new System.NullReferenceException("Failed to init");
				myView?.Show(_card);
			}
		}

		private DeckBuilderTopLeftCardView? topLeftCardView;
		private DeckBuilderCardView? myView;

		public override void _Ready()
		{
			_ = InfoDisplayer ?? throw new System.NullReferenceException("Forgot to init");
			myView = new DeckBuilderCardView(InfoDisplayer);
		}

		public void Init(DeckBuilderCard card, DeckBuilderTopLeftCardView topLeftCardView, DeckBuilderDeckController deckController)
		{
			DeckController = deckController;

			this.topLeftCardView = topLeftCardView;

			Card = card;
		}

		public void Clicked(InputEvent input)
		{
			if (input is InputEventMouseButton mouseInput) HandleMouseEvent(mouseInput);
		}

		protected virtual void HandleMouseEvent(InputEventMouseButton mouseInput)
		{
			if (mouseInput.ButtonIndex == MouseButton.Left && mouseInput.DoubleClick) DoubleLeftClick();
		}

		protected virtual void DoubleLeftClick()
		{
			_ = DeckController ?? throw new System.NullReferenceException("Forgot to init");
			DeckController.BecomeAvatar(this);
		}

		public void ShowInTopLeft()
		{
			_ = topLeftCardView ?? throw new System.NullReferenceException("Forgot to init");
			topLeftCardView.Show(Card);
		}
	}
}