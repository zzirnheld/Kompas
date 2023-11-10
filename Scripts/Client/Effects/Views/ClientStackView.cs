using Godot;
using Kompas.Client.Effects.Models;
using Kompas.Client.UI;

namespace Kompas.Client.Effects.Views
{
	public partial class ClientStackView : Node 
	{
		[Export]
		private CurrentStateController CurrentStateController { get; set; }

		//Placeholders just to show for debugging. Eventually, probably want osmething like a pulse/glow effect around what's activating
		//TODO also should show the actual stack, ideally in a scrollable way
		public void Activated(IClientStackable stackable)
		{
			CurrentStateController.ShowCurrentStateInfo($"Activated {stackable.StackableBlurb}");
		}

		public void Resolving(IClientStackable stackable)
		{
			CurrentStateController.ShowCurrentStateInfo($"Resolving {stackable.StackableBlurb}");
		}
	}
}