using Godot;
using Kompas.Godot;
using Kompas.Shared.Exceptions;
using Kompas.UI.DeckBuilder;
using System;

namespace Kompas.Shared.Controllers
{
	public partial class OldEscapeMenuController : Node
	{
		//Name is for historical reasons.
		//TODO rename
		[Export]
		private DeckBuilderEscapeMenuLogo? _logoController;
		private DeckBuilderEscapeMenuLogo LogoController => _logoController ?? throw new UnassignedReferenceException(nameof(_logoController));

		[Export]
		private PackedScene? _menuButton;
		private PackedScene MenuButton => _menuButton ?? throw new UnassignedReferenceException(nameof(_menuButton));

		[Export]
		private Control? _buttonsParent;
		private Control ButtonsParent => _buttonsParent ?? throw new UnassignedReferenceException(nameof(_buttonsParent));

		public readonly struct ButtonData
		{
			public string Text { get; init; }
			public Action OnClick { get; init; }
		}

		private bool initialized = false;

		public void Init(params ButtonData[] buttonsData)
		{
			if (initialized) throw new AlreadyInitializedException();
			initialized = true;

			foreach (var buttonData in buttonsData)
			{
				var button = MenuButton.Instantiate<Button>();
				
				button.Text = buttonData.Text;
				button.Pressed += buttonData.OnClick;
				button.MouseEntered += () => LogoController.LookTowards(button.GlobalCenter());

				ButtonsParent.AddChild(button);
			}
		}
	}
}