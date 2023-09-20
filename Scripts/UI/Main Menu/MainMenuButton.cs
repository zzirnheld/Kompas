using Godot;

namespace Kompas.UI.MainMenu
{
	public partial class MainMenuButton : Button
	{
		[Export]
		public RotatingTextureRect SpinningLogo { get; set; }

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			MouseEntered += () =>
			{
				var targetPosition = GlobalPosition + (Size / 2);
				GD.Print($"Look towards {GlobalPosition} + {(Size / 2)} = {targetPosition}!");
				SpinningLogo.LookTowards(targetPosition);
			};
		}
	}
}