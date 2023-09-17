using Godot;
using Kompas.Cards.Models;

namespace Kompas.UI.CardInfoDisplayers.DeckBuilder
{
	public partial class DeckBuilderBuiltDeckAvatarInfoDisplayer : DeckBuilderBuiltDeckInfoDisplayer
	{
		[Export]
		private Texture2D FallbackAvatarTexture { get; set; }

		public override void DisplayCardRulesText(CardBase card)
		{
			//Do I want to display its name?
		}

		public void Clear()
		{
			Texture = FallbackAvatarTexture;
		}
	}
}