using System.Collections.Generic;
using Godot;

namespace Kompas.UI.TextBehavior
{
	public partial class ShrinkRichTextOnOverrun : RichTextLabel
	{
		private const string FontSizeName = "normal_font_size";

		private int fontSize;

		public string ShrinkableText
		{
			set
			{
				RemoveThemeFontSizeOverride(FontSizeName);
				fontSize = GetThemeDefaultFontSize();
				Text = value;
				AddThemeFontSizeOverride(FontSizeName, fontSize);
			}
		}

		public override void _Process(double delta)
		{
			if (GetVisibleLineCount() < GetLineCount())
			{
				RemoveThemeFontSizeOverride(FontSizeName);
				fontSize--;
				AddThemeFontSizeOverride(FontSizeName, fontSize);
			}
		}
	}
}