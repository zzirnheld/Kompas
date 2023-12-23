using Godot;
using Kompas.Shared.Controllers;
using System;
using System.Linq;

public partial class TestSpiral : Node3D
{
	[Export]
	private SpiralController? Spiral { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Spiral?.SpiralOut(GetChildren().Cast<Node3D>().ToArray());
	}
}
