using Godot;
using System;

public partial class MainMenuButton : Button
{
	[Export]
	public MainMenuKompasLogo SpinningLogo { get; set; }

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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
