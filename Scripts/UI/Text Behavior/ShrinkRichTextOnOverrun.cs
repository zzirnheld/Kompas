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


		public override void _Ready()
		{
			VisibilityChanged += ReshowText;
			Resized += ReshowText;

			MetaHoverStarted += metadata => { GD.Print($"{Name} start {metadata}, {metadata.GetType()}"); };
			MetaHoverEnded += metadata => { GD.Print($"{Name} end {metadata}, {metadata.GetType()}"); };
			MetaClicked += metadata => { GD.Print($"{Name} click {metadata}, {metadata.GetType()}"); };
		}

		public void ReshowText()
		{
			//Guard against infinite recursion
			if (currentlyResizingText) return;
			currentlyResizingText = true;
			ShrinkableText = Text;
			currentlyResizingText = false;
		}

		public string ShrinkableText //TODO crop out tags when determining "size"
		{
			set
			{
				//GD.Print($"Shrinkable rich text set to {Text}");
				if (!IsVisibleInTree() || Size.Y == 0)
				{
					GD.Print($"Not properly visible yet, not resizing rich text {Name} for overrun. Visible in tree? {IsVisibleInTree()} Y? {Size.Y}");
					Text = value;
					return;
				}

				RemoveThemeFontSizeOverride(FontSizeName);
				Font font = GetThemeDefaultFont();
				int nextFontSizeToTry = UseThemeDefaultFontSize ? GetThemeDefaultFontSize() : StartingFontSize;
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