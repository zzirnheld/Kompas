using Godot;
using Kompas.Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.UI.MainMenu
{
	public partial class MainMenuButton : Button
	{
		[Export]
		private MainMenuLogoController? _spinningLogo;
		public MainMenuLogoController SpinningLogo => _spinningLogo
			?? throw new UnassignedReferenceException();

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			MouseEntered += () => SpinningLogo.LookTowards(this);
		}
	}
}