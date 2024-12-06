using Godot;
using Kompas.Shared.Exceptions;

namespace Kompas;

public partial class LinkMeshToViewportTexture : MeshInstance3D
{
	[Export]
	private SubViewport? _subViewport;
	private SubViewport SubViewport => _subViewport
		?? throw new UnassignedReferenceException(nameof(_subViewport), this);


	public override void _Ready()
	{
		base._Ready();
		var material = new StandardMaterial3D
		{
			Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
			AlbedoTexture = SubViewport.GetTexture()
		};
		SetSurfaceOverrideMaterial(0, material);
	}

}

