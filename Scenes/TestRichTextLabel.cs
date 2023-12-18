using Godot;
using System;

public partial class TestRichTextLabel : RichTextLabel
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MetaHoverStarted += metadata => { GD.Print($"start {metadata}, {metadata.GetType()}"); };
		MetaHoverEnded += metadata => { GD.Print($"end {metadata}, {metadata.GetType()}"); };
		MetaClicked += metadata => { GD.Print($"click {metadata}, {metadata.GetType()}"); };
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
