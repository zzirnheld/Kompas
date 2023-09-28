using Godot;
using System;

namespace Kompas.UI.DeckBuilder
{
	public partial class SaveAsController : Control
	{
		[Export]
		private DeckBuilderDeckController DeckBuilderDeckController { get; set; }

		[Export]
		private LineEdit DeckNameEdit { get; set; }

		public void Enable()
		{
			DeckBuilderDeckController.ShowController(DeckBuilderDeckController.Tab.NewDeck);
		}

		public void Confirm()
		{
			DeckBuilderDeckController.NewDeck(DeckNameEdit.Text);
			DeckBuilderDeckController.ShowController(DeckBuilderDeckController.Tab.Normal);
		}

		public void Cancel()
		{
			DeckBuilderDeckController.ShowController(DeckBuilderDeckController.Tab.Normal);
		}
	}
}