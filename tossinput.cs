using Godot;
using System;

public partial class tossinput : Control
{
	// [Export]
	// public MeshInstance3D mesh;
	// [Export]
	// public SubViewport viewport;

	// // Called when the node enters the scene tree for the first time.
	// public override void _Ready()
	// {
	// }

	// // Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// }

	// public override void _Input(InputEvent @event)
	// {
	// 	base._Input(@event);
	// 	if (@event is not InputEventMouse mie) return;

	// 	GD.Print($"t{Name}: {mie.Position}");
	// 	//GD.Print(@event);
	// 	var dupe = mie.Duplicate();
	// 	if (dupe is not InputEventMouse pass)
	// 	{
	// 		GD.Print($"Instead of being {mie.GetType()}, was {dupe.GetType()}");
	// 		return;
	// 	}
	// 	var pos = viewport.GetMousePosition();
	// 	//var pos = viewport.GlobalCanvasTransform.AffineInverse() * pass.Position;
	// 	pass.Position = pos;
	// 	GD.Print($"tPassing {pass.Position}");
	// 	viewport?.PushInput(@pass, true);
	// }
}
