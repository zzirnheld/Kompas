using Godot;

namespace Kompas.UI;

public partial class RightJustifiedOptionButton : OptionButton
{
	public override void _Ready()
	{
		Alignment = HorizontalAlignment.Right;
	}
}