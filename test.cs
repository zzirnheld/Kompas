using Godot;
using System;

public partial class test : RichTextLabel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("start");
		MetaHoverStarted += (str) => GD.PrintErr("chai!" + str + "\n\n\n\n\n\n\n\n\n");
		MouseEntered += () => GD.PrintErr("yes");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// public override void _Input(InputEvent @event)
	// {
	// 	base._Input(@event);
	// 	GD.Print($"1input event on {Name}: {@event}");
	// 	if (@event is not InputEventMouse mie) return;

	// 	GD.Print($"1{Name}: {mie.Position}");
	// }
}
