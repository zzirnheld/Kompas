using Godot;

namespace Kompas.UI.TextBehavior
{
	public partial class ShrinkOnOverrun : Label
	{
		private const string FontSizeName = "font_size";

		[Export]
		private bool UseThemeDefaultFontSize { get; set; } = false;

		[Export]
		private int StartingFontSize { get; set; } = 20;

		private bool currentlyResizingText = false;

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
			ShrinkableText = Text;
			currentlyResizingText = false;
		}

		public string ShrinkableText
		{
			set
			{
				if (!IsVisibleInTree() || Size.Y == 0 || Size.X == 0)
				{
					GD.Print($"Not properly visible yet, not resizing text {Name} for overrun");
					Text = value;
					return;
				}

				RemoveThemeFontSizeOverride(FontSizeName);

				Font font = GetThemeDefaultFont();
				int fontSize = UseThemeDefaultFontSize ? GetThemeDefaultFontSize() : StartingFontSize;
				while (font.GetStringSize(value, fontSize: fontSize).X > Size.X) fontSize--;

				AddThemeFontSizeOverride(FontSizeName, fontSize);
				Text = value;
			}
		}
	}
}