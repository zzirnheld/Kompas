using Godot;
using System;

public partial class MainMenuKompasLogo : TextureRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Resize();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Rotation += 1f * (float)delta;
	}

	public void Resize()
	{
		GD.Print($"Resizing. Size is {Size}");
		PivotOffset = Size / 2;
	}
}
