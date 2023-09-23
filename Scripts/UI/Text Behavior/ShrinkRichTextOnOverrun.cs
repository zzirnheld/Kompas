using Godot;

namespace Kompas.UI.TextBehavior
{
	public partial class ShrinkRichTextOnOverrun : RichTextLabel
	{
		private const string FontSizeName = "font_size";

		private float defaultHeight;

		public override void _Ready()
		{
			defaultHeight = Size.Y;
		}
		//TODO also update on resize? if window size changes

		public string ShrinkableText
		{
			set
			{
				RemoveThemeFontSizeOverride(FontSizeName);

				int fontSize = GetThemeDefaultFontSize();
				Text = value;
				while (Size.Y > defaultHeight)
				{
					RemoveThemeFontSizeOverride(FontSizeName);
					fontSize--;
					AddThemeFontSizeOverride(FontSizeName, fontSize);
					Text = value;
				}
			}
		}
	}
}