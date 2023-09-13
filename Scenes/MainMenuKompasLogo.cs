using Godot;
using System;

public partial class MainMenuKompasLogo : TextureRect
{
	[Export]
	public Control center;

	private float RotateQuarterCounterclockwise = -Mathf.Pi / 2f;

	private float targetRotation;
	//private float startRotation;
	//private float currentRotationalVelocity;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Resize()
	{
		GD.Print($"Resizing. Size is {Size}");
		PivotOffset = Size / 2;
	}

	public void LookTowards(Vector2 targetPosition)
	{
		var currentPosition = center.GlobalPosition;
		float tan = (targetPosition.Y - currentPosition.Y)
				  / (targetPosition.X - currentPosition.X);
		targetRotation = Mathf.Atan(tan) + RotateQuarterCounterclockwise; //To have angle start from horizontal
		GD.Print($"from {currentPosition} to {targetPosition} tan is {tan}, so target rotation {targetRotation}");
		Rotation = targetRotation;
	}
}
