using Godot;
using Kompas.Godot;
using Kompas.Shared.Exceptions;
using Kompas.UI.MainMenu;

namespace Kompas.Client.UI;

public partial class CameraFollowObject : Camera3D
{
	[Export]
	private Node3D? _ring;
	private Node3D Ring => _ring
		?? throw new UnassignedReferenceException(nameof(_ring), this);

	private Node3D? follow;

	private const float LerpTime = 0.25f;

	// private Vector3 originPosition;
	// private Quaternion originRotation;
	private double lerp = 0f;

	private Transform3D cameraOriginGlobalTransform;
	private Transform3D ringOriginLocalTransform;
	private static readonly Transform3D ringDestLocalTransform = new(new Basis(Quaternion.Identity), Vector3.Zero);

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (follow == null) return;

		lerp += delta / LerpTime;
		//GD.Print($"{lerp} -> {Shared.Math.Cubic((float)lerp)}");
		var progress = Shared.Math.Cubic((float) Mathf.Clamp(lerp, 0f, 1f));
		//lerp = Shared.Math.CubicProgress((float)lerp);
		// GlobalPosition = originPosition.Lerp(follow.GlobalPosition, (float)lerp);
		// GlobalBasis = new Basis(originRotation.Slerp(follow.GlobalBasis.GetRotationQuaternion(), (float) lerp));

		GlobalTransform = cameraOriginGlobalTransform.InterpolateWith(follow.GlobalTransform, progress);

		Ring.Transform = ringOriginLocalTransform.InterpolateWith(ringDestLocalTransform, progress);

		//GD.Print($"Matching {follow.GlobalPosition}, {follow.GlobalRotation}");
	}

	public void Follow(Node3D node, uint layerMask)
	{
		//GD.Print($"Following {follow} with mask {layerMask}");
		if (node != follow) lerp = 0;
		else lerp = 1f;

		//TODO: want the motion to appear continuous as it changes the focusing of the card, so... maybe do some offsetting of the origin position/basis?
		//Will need to do math so that the offset accounts for the current rotation...
		//Once I figure out that math, I can remove the else case above to have motion appear to "finish"

		//Might also/instead want to make it a cubic interpolation to maybe ease off on the stutteriness if you go from one card to another in a grid while having a card highlighted.
		//still, really good spot for now.

		follow = node;
		CullMask = layerMask;
		// originPosition = GlobalPosition;
		// originRotation = GlobalBasis.GetRotationQuaternion();
		cameraOriginGlobalTransform = GlobalTransform;

		var ringGlobal = Ring.GlobalTransform;
		node.TransferChild(Ring);
		Ring.GlobalTransform = ringGlobal;
		ringOriginLocalTransform = Ring.Transform;
	}
}

