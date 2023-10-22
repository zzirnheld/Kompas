using Godot;
using Kompas.Client.Gamestate;
using System;

namespace Kompas.Client.UI
{
	public partial class EndTurnButton : Button
	{
		[Export]
		private ClientGameController GameController { get; set; }

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Pressed += EndTurn;
		}

		private void EndTurn()
		{
			GameController.Notifier.RequestEndTurn();
		}
	}
}