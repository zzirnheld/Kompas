using Godot;
using Kompas.Shared.Exceptions;
using System;
using System.Linq;

namespace Kompas.UI.DeckBuilder
{
	public partial class NewDeckController : Control
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
			DeckBuilderDeckController.ShowController(DeckBuilderDeckController.Tab.NewDeck);
		}

		public void Confirm()
		{
			string deckName = DeckNameEdit.Text;
			DeckNameEdit.Text = null;
			if (!AllowedDeckName(deckName))
			{
				GD.PushError($"{deckName} is an invalid deck name!");
				return;
			};
			DeckBuilderDeckController.NewDeck(deckName ?? string.Empty);
			DeckBuilderDeckController.ShowController(DeckBuilderDeckController.Tab.Normal);
		}

		private static bool AllowedDeckName(string name) => name != string.Empty && name.All(char.IsLetterOrDigit);

		public void Cancel()
		{
			DeckBuilderDeckController.ShowController(DeckBuilderDeckController.Tab.Normal);
		}
	}
}