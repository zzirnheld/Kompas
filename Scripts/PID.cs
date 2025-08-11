using Godot;

namespace Kompas
{
	public partial class PID : Label
	{
		public override void _Ready()
		{
			base._Ready();
			Text = $"{OS.GetProcessId()}";
		}
	}
}

