using Godot;

namespace Kompas
{
	public partial class test2 : SubViewport
	{
		[Export]
		public MeshInstance3D mesh;
		[Export]
		public Viewport nestedPort;

		public override void _Ready()
		{
			base._Ready();
			var material = new StandardMaterial3D();
			material.AlbedoTexture = GetTexture();
			mesh?.SetSurfaceOverrideMaterial(0, material);
		}
	}
}

