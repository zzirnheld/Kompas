using System;
using Godot;
using Kompas.Shared.Exceptions;

namespace Kompas.UI.MainMenu
{
	public partial class SplashScreenLogo : Control
	{
		[Export]
		private RotatingTextureRect? _rotatingTextureRect;
		public RotatingTextureRect RotatingTextureRect => _rotatingTextureRect
			?? throw new UnassignedReferenceException();
	}
}