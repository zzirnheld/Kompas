using Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.Client.UI;

public partial class ForwardingTextureRect : TextureRect
{
	private const string ColliderIntersectionKey = "collider";
	private const string PositionIntersectionKey = "position";
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

	//When the player mouses over the texture rect, we want to inform the area it's looking at that it's being moused over, too
	public override void _GuiInput(InputEvent @event)
	{
		if (@event is not InputEventMouse iem) return;
		var posMyCoords = (iem.Position - this.Position);
		var posTheirCoords = new Vector2(posMyCoords.X * (SubViewport.Size.X / Size.X), posMyCoords.Y * (SubViewport.Size.Y / Size.Y));

		DoRaycast(ViewportCamera, posTheirCoords);
	}

	private static void DoRaycast(Camera3D cameraParam, Vector2 positionInViewport)
	{
		var from = cameraParam.ProjectRayOrigin(positionInViewport);
		var to = from + cameraParam.ProjectRayNormal(positionInViewport) * 1000.0f;

		var query = PhysicsRayQueryParameters3D.Create(from, to);
		query.CollideWithAreas = true;

		var spaceState = cameraParam.GetWorld3D().DirectSpaceState;
		var intersections = spaceState.IntersectRay(query);

		if (intersections.Count < 1) return;

		var collided = intersections[ColliderIntersectionKey].As<Node>();
		if (collided is not Area3DAroundViewportQuad area) return;

		var position = intersections[PositionIntersectionKey].AsVector3();
		area.HandleCameraRayToHere(position);
	}
}
