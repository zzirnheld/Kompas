using Godot;
using System;
using System.Linq;

namespace Kompas.UI.DeckBuilder
{
	public partial class NewDeckController : Control
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
			string deckName = DeckNameEdit.Text;
			DeckNameEdit.Text = null;
			if (!AllowedDeckName(deckName)) return;
			DeckBuilderDeckController.NewDeck(DeckNameEdit.Text);
			DeckBuilderDeckController.ShowController(DeckBuilderDeckController.Tab.Normal);
		}

		private static bool AllowedDeckName(string name) => name != string.Empty && name.All(char.IsLetterOrDigit);

		public void Cancel()
		{
			DeckBuilderDeckController.ShowController(DeckBuilderDeckController.Tab.Normal);
		}
	}
}