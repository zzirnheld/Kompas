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

			if (@event is not InputEventMouse iem) return;
			var pos = iem.Position - this.Position + (this.viewport.Size / 2);
			var pass = @event.Duplicate() as InputEventMouse;
			pass.Position = pos;
			GD.Print($"For that event, {GetViewport().GetMousePosition()} vs {viewport.GetMousePosition()} vs {this.viewport.GetMousePosition()} vs {pos}");

			this.viewport.PushInput(pass);

			this.viewport.HandleInputLocally = false;
		}
	}
}

