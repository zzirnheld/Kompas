using Godot;

namespace Kompas.Client.UI;

public partial class CameraFollowObject : Camera3D
{
	private Node3D? follow;

	private const float LerpTime = 1f;

	private Vector3 originPosition;
	private Quaternion originRotation;
	private double lerp = 0f;

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (follow == null) return;

		lerp += delta;
		lerp = Mathf.Clamp(lerp, 0f, 1f);
		GlobalPosition = originPosition.Lerp(follow.GlobalPosition, (float)lerp);
		GlobalBasis = new Basis(originRotation.Slerp(follow.GlobalBasis.GetRotationQuaternion(), (float) lerp));


		//GD.Print($"Matching {follow.GlobalPosition}, {follow.GlobalRotation}");
	}

	public void Follow(Node3D node)
	{
		lerp = 0f;
		follow = node;
		originPosition = GlobalPosition;
		originRotation = GlobalBasis.GetRotationQuaternion();
	}
}

