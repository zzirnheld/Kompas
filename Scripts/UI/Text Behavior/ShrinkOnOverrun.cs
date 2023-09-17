using Godot;

namespace Kompas.UI.TextBehavior
{
	public partial class ShrinkOnOverrun : Label
	{
		private const string FontSizeName = "font_size";

		public string ShrinkableText
		{
			set
			{
				RemoveThemeFontSizeOverride(FontSizeName);

				Font font = GetThemeDefaultFont();
				int fontSize = GetThemeDefaultFontSize();
				while (font.GetStringSize(value, fontSize: fontSize).X > Size.X) fontSize--;

				AddThemeFontSizeOverride(FontSizeName, fontSize);
				Text = value;
			}
		}
	}
}