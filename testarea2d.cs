using Godot;

namespace Kompas
{
	public partial class testarea2d : Area2D
	{
		[Export]
		public SubViewport viewport;

		public override void _Input(InputEvent @event)
		{
			base._Input(@event);
			if (@event is InputEventMouse || @event is InputEventScreenDrag || @event is InputEventScreenTouch) return;

			GD.Print($"{Name} is gonna push non input event {@event}");
			viewport.PushInput(@event);
		}

		public override void _InputEvent(Viewport viewport, InputEvent @event, int shapeIdx)
		{
			this.viewport.HandleInputLocally = true;

			GD.Print($"{Name} is gonna push input event {@event} from {new System.Exception().StackTrace}");
			this.viewport.PushInput(@event.Duplicate() as InputEvent);

			this.viewport.HandleInputLocally = false;
		}
	}
}

