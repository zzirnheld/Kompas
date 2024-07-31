using System.Linq;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Client.UI;
using Kompas.Shared.Exceptions;

namespace Kompas;

/// <summary>
/// Drawn from a godot forum help post.
/// I've... lost track of the post, though.
/// Doing my best to document what I've learned as I mess with this.
///</summary>
public partial class Area3DAroundViewportQuad : Area3D
{
	[Export]
	private MeshInstance3D? _quadMesh;
	/// <summary>
	/// This MUST be a quad mesh to work correctly, as far as I can tell
	///</summary>
	private MeshInstance3D QuadMesh => _quadMesh
		?? throw new UnassignedReferenceException(nameof(_quadMesh), this);

	[Export]
	private SubViewport? _subViewport;
	/// <summary>
	/// Subviewport that <see cref="QuadMesh"/> has a ViewportTexture of.
	///</summary>
	public SubViewport SubViewport => _subViewport
		?? throw new UnassignedReferenceException(nameof(_subViewport), this);

	/// <summary>
	/// A raycast from some camera (this class doesn't care which) found this area.
	/// When that happens, we propagate that as a mouse motion event into the given viewport.
	///</summary>
	public void HandleCameraRayToHere(Vector3 eventPos)
	{
		if (QuadMesh.Mesh is not PlaneMesh planeMesh) throw new System.InvalidOperationException("MUST be a plane mesh");

		var quadMeshSize = planeMesh.Size;

		//Start with the event's position, transformed by the global affine inverse.
		//I don't remember enough linear algebra to remember what this does, so ask Jerry later
		//What I'm *pretty* sure it does is invert the transform to take the event position in local space and convert it to global space
		var pos = GlobalTransform.AffineInverse() * eventPos;

		//Then, we need "screen coords", so we're gonna have to grab two coords - in this case, the x and -y ones (because y descends from the top of the screen. I think.)
		var destPos = new Vector2(pos.X, -pos.Y);

		//Divide by the size of the mesh, to get a (-0.5, 0.5) number of where we are w/r/t the center of the mesh
		destPos.X /= quadMeshSize.X;
		destPos.Y /= quadMeshSize.Y;

		//Add 0.5 to offset to a (0, 1) range value
		destPos.X += 0.5f;
		destPos.Y += 0.5f;

		//Multiply by the viewport's size, to go from (0, 1) to (0, size of viewport)
		destPos.X *= SubViewport.Size.X;
		destPos.Y *= SubViewport.Size.Y;

		//Duplicate the event, to ensure we don't corrupt any info on the event previously.
		var duplicate = new InputEventMouseMotion();
		//Set the new event position only on the duplicate
		duplicate.Position = destPos;

		//Finally, we can push the duplicate event with the adjusted coords in the viewport's space!
		SubViewport.PushInput(duplicate, true);
	}
	
}

