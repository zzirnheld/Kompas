using Godot;

namespace Kompas;

public partial class test3 : Area3D
{
	[Export]
	public MeshInstance3D mesh;
	[Export]
	public SubViewport nestedPort;

	public void hello() => GD.Print("hello");

	public override void _Ready()
	{
		base._Ready();
		this.InputEvent += iiiiinput;
	}

	public void iiiiinput(Node camera, InputEvent @event, Vector3 eventPos, Vector3 normal, long shapeIdx)
	{
		//GD.Print(@event.GetType());
		if (@event is not InputEventMouse mie) return;
		GD.Print($"3{Name}: {mie.Position}");

		var quadMeshSize = (mesh.Mesh as PlaneMesh).Size;
		//GD.Print($"3mesh size {quadMeshSize}");

		var pos = GlobalTransform.AffineInverse() * eventPos;

		var destPos = new Vector2();

		destPos = new(pos.X, -pos.Y);

		destPos.X = destPos.X / quadMeshSize.X;
		destPos.Y = destPos.Y / quadMeshSize.Y;

		destPos.X += 0.5f;
		destPos.Y += 0.5f;

		destPos.X *= nestedPort.Size.X;
		destPos.Y *= nestedPort.Size.Y;

		//GD.Print(@event);
		var dupe = mie.Duplicate();
		if (dupe is not InputEventMouse pass)
		{
			GD.Print($"Instead of being {mie.GetType()}, was {dupe.GetType()}");
			return;
		}

		pass.Position = destPos;
		GD.Print($"{eventPos} -> {pos} -> {destPos} -> {pass.Position}");

		nestedPort.PushInput(pass, true);
	}
	
}

