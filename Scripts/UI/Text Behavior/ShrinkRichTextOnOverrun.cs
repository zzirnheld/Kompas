using Godot;

namespace Kompas.UI.TextBehavior
{
	public partial class ShrinkRichTextOnOverrun : RichTextLabel
	{
		private const int MinFontSize = 8;

		private const string FontSizeName = "normal_font_size";


		public override void _Ready()
		{
			VisibilityChanged += ReshowText;
			Resized += ReshowText;
		}

		public void ReshowText()
		{
			ShrinkableText = Text;
		}

		public string ShrinkableText
		{
			set
			{
				GD.Print($"Shrinkable rich text set to {Text}");
				if (!IsVisibleInTree() || Size.Y == 0)
				{
					GD.Print("Not properly visible yet, not resizing rich text for overrun");
					Text = value;
					return;
				}

				RemoveThemeFontSizeOverride(FontSizeName);
				Font font = GetThemeDefaultFont();
				int nextFontSizeToTry = GetThemeDefaultFontSize();
				float height = float.MaxValue;
				float targetHeight = Size.Y;

				while (height > targetHeight && nextFontSizeToTry > MinFontSize)
				{
					height = font.GetMultilineStringSize(value, width: Size.X, fontSize: nextFontSizeToTry).Y;
					AddThemeFontSizeOverride(FontSizeName, nextFontSizeToTry);
					nextFontSizeToTry--;
				}

				if (nextFontSizeToTry <= MinFontSize) GD.PrintErr($"{value} is too long, boiiii");

				Text = value;
			}
		}
	}
}