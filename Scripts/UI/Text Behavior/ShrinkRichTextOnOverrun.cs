using System.Collections.Generic;
using Godot;

namespace Kompas.UI.TextBehavior
{
	public partial class ShrinkRichTextOnOverrun : RichTextLabel
	{
		private const string FontSizeName = "normal_font_size";

		public string ShrinkableText
		{
			set
			{
				RemoveThemeFontSizeOverride(FontSizeName);
				Font font = GetThemeDefaultFont();
				int nextFontSizeToTry = GetThemeDefaultFontSize();
				float height = float.MaxValue;

				while (height > Size.Y)
				{
					height = font.GetMultilineStringSize(value, width: Size.X, fontSize: nextFontSizeToTry).Y;
					AddThemeFontSizeOverride(FontSizeName, nextFontSizeToTry);
					nextFontSizeToTry--;
				}

				Text = value;
			}
		}
	}
}