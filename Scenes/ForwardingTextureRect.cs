using Godot;
using Kompas.Shared.Exceptions;
using System;

namespace Kompas.Client.UI;

public partial class ForwardingTextureRect : TextureRect
{
	[Export]
	private SubViewport? _subViewport;
	private SubViewport SubViewport => _subViewport
		?? throw new UnassignedReferenceException(nameof(_subViewport), this);

	[Export]
	private Camera3D? _viewportCamera;
	private Camera3D ViewportCamera => _viewportCamera
		?? throw new UnassignedReferenceException(nameof(_viewportCamera), this);

	private bool mouseInside;

	public override void _Ready()
	{
		base._Ready();
		MouseEntered += () => mouseInside = true;
		MouseExited += () => mouseInside = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// public override void _Process(double delta)
	// {
	// 	if (!mouseInside) return;

	// 	var destPos = GetLocalMousePosition() - GlobalPosition;// + (Size / 2);

	// 	//Duplicate the event, to ensure we don't corrupt any info on the event previously.
	// 	var duplicate = new InputEventMouseMotion();
	// 	//Set the new event position only on the duplicate
	// 	duplicate.Position = destPos;
	// 	GD.Print($"Forwarding to {GetLocalMousePosition()} - {GlobalPosition} maybe + {Size / 2} = {destPos}");

	// 	//Finally, we can push the duplicate event with the adjusted coords in the viewport's space!
	// 	SubViewport.PushInput(duplicate, true);
	// }

	public override void _GuiInput(InputEvent @event)
	{
		GD.Print($"{Name} is gonna push input event {@event} from {new System.Exception().StackTrace}");

		if (@event is not InputEventMouse iem) return;
		var posMyCoords = (iem.Position - this.Position);
		var posTheirCoords = new Vector2(posMyCoords.X * (SubViewport.Size.X / Size.X), posMyCoords.Y * (SubViewport.Size.Y / Size.Y));
		GD.Print($"For that event, {GetViewport().GetMousePosition()} vs {SubViewport.GetMousePosition()} vs {posTheirCoords}");

		DoRaycast(ViewportCamera, posTheirCoords);
	}

	private void DoRaycast(Camera3D cameraParam, Vector2 positionInViewport)
	{
		var spaceState = cameraParam.GetWorld3D().DirectSpaceState;

		var from = cameraParam.ProjectRayOrigin(positionInViewport);
		var to = from + cameraParam.ProjectRayNormal(positionInViewport) * 1000.0f;

		var query = PhysicsRayQueryParameters3D.Create(from, to);
		query.CollideWithAreas = true;
		var intersections = spaceState.IntersectRay(query);
		GD.Print($"Forwarding Casting from {from} to {to}, intersections? {intersections.Count}");

		if (intersections.Count < 1) return;

		//GD.Print($"{intersections["collider"]} is a {intersections["collider"].GetType()}");
		var collided = intersections["collider"].As<Node>();
		if (collided is not Area3DAroundViewportQuad area) return;

		var position = intersections["position"].AsVector3();
		//GD.Print($"Intersected {collided} from {this} at {position}");
		area.HandleRayToHere(position);
	}
}
