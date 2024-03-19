using Godot;
using System;

namespace Kompas.Gamestate.Locations.Controllers
{
	/// <summary>
    /// This node should ONLY have children that are the Area3Ds for the spaces and they should be in order
    /// </summary>
	public partial class SpacesClickingController : Node
	{
		public event EventHandler<Space>? LeftClick;

		public override void _Ready()
		{
			int x = 0, y = 0;
			foreach (var child in GetChildren())
			{
				if (child is not Area3D area) continue;

				area.InputEvent += CreateInputEventHandler((x, y));
				x++;
				if (x == Space.BoardLen)
				{
					x = 0;
					y++;
				}
			}

			LeftClick += (_, space) => GD.Print($"Clicked {space}");
		}

		private CollisionObject3D.InputEventEventHandler CreateInputEventHandler(Space space)
		{
			return (Node camera, InputEvent inputEvent, Vector3 position, Vector3 normal, long shapeIdx) =>
			{
				if (inputEvent is not InputEventMouseButton mouseEvent)
				{
					return;
				}

				//Event where now the mouseEvent is Pressed means it's when the mouse goes down
				if (mouseEvent.Pressed && mouseEvent.ButtonIndex == MouseButton.Left)
				{
					LeftClick?.Invoke(this, space);
				}
			};
		}
	}
}