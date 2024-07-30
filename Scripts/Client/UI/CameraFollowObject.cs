using Godot;

namespace Kompas.Client.UI;

public partial class CameraFollowObject : Camera3D
{
	private Node3D? follow;

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (follow == null) return;

		GlobalPosition = follow.GlobalPosition;
		//GlobalBasis = follow.GlobalBasis;

		//GD.Print($"Matching {follow.GlobalPosition}, {follow.GlobalRotation}");
	}

	public void Follow(Node3D node)
	{
		follow = node;
	}
}

