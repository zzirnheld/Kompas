using Godot;
using Kompas.Shared.Exceptions;
using System;

namespace Kompas.UI.DeckBuilder
{
	public partial class SaveAsController : Control
	{
		[Export]
		private DeckBuilderDeckController? _deckBuilderDeckController;
		private DeckBuilderDeckController DeckBuilderDeckController => _deckBuilderDeckController
			?? throw new UnassignedReferenceException();

		[Export]
		private LineEdit? _deckNameEdit;
		private LineEdit DeckNameEdit => _deckNameEdit
			?? throw new UnassignedReferenceException();

		public void Enable()
		{
			DeckBuilderDeckController.ShowController(DeckBuilderDeckController.Tab.SaveAs);
		}

		public void Confirm()
		{
			DeckBuilderDeckController.SaveAs(DeckNameEdit.Text);
			DeckBuilderDeckController.ShowController(DeckBuilderDeckController.Tab.Normal);
		}

		public void Cancel()
		{
			DeckBuilderDeckController.ShowController(DeckBuilderDeckController.Tab.Normal);
		}
	}
}