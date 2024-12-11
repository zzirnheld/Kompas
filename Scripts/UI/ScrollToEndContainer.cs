using Godot;

namespace Kompas.UI;

public partial class ScrollToEndContainer : ScrollContainer
{
	public override void _Ready()
	{
		base._Ready();
		var scrollBar = GetVScrollBar();
		scrollBar.Changed += () =>
		{
			if (scrollBar.MaxValue != scrollBar.Value) scrollBar.Value = scrollBar.MaxValue;
		};
	}
}