using Godot;

namespace Kompas.UI.TextBehavior
{
	public partial class ShrinkRichTextOnOverrun : RichTextLabel
	{
		private const int MinFontSize = 8;

		private const string FontSizeName = "normal_font_size";

		[Export]
		private bool UseThemeDefaultFontSize { get; set; } = false;

		[Export]
		private int StartingFontSize { get; set; } = 19;

		private bool currentlyResizingText = false;
		private string rawText = string.Empty;


		public override void _Ready()
		{
			VisibilityChanged += ReshowText;
			Resized += ReshowText;
		}

		public void ReshowText()
		{
			//Guard against infinite recursion
			if (currentlyResizingText) return;
			currentlyResizingText = true;
			SetShrinkableText(rawText, Text);
			currentlyResizingText = false;
		}

		/// <summary>
        /// Sets the shrinkable text
        /// </summary>
        /// <param name="text">The text that we should size based off of</param>
        /// <param name="bbCodeText">The BBCode text that we should actually display (but includes tags we should ignore)</param>
		public void SetShrinkableText(string text, string bbCodeText)
		{
			//GD.Print($"Shrinkable rich text set to {Text}");
			if (!IsVisibleInTree() || Size.Y == 0)
			{
				GD.Print($"Not properly visible yet, not resizing rich text {Name} for overrun. Visible in tree? {IsVisibleInTree()} Y? {Size.Y}");
				Text = bbCodeText;
				rawText = text;
				return;
			}

			RemoveThemeFontSizeOverride(FontSizeName);
			Font font = GetThemeDefaultFont();
			int nextFontSizeToTry = UseThemeDefaultFontSize ? GetThemeDefaultFontSize() : StartingFontSize;
			float height = float.MaxValue;
			float targetHeight = Size.Y;

			while (height > targetHeight && nextFontSizeToTry > MinFontSize)
			{
				height = font.GetMultilineStringSize(text, width: Size.X, fontSize: nextFontSizeToTry).Y;
				AddThemeFontSizeOverride(FontSizeName, nextFontSizeToTry);
				nextFontSizeToTry--;
			}

			if (nextFontSizeToTry <= MinFontSize) GD.PrintErr($"{text} is too long, boiiii");

			Text = bbCodeText;
			rawText = text;
		}
	}
}