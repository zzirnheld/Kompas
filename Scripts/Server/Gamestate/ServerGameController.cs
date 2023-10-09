using Godot;
using Kompas.Gamestate;
using Kompas.Server.Cards.Loading;
using System;

namespace Kompas.Server.Gamestate
{
	public partial class ServerGameController : GameController
	{
		public ServerGame ServerGame { get; private set; }
		public override IGame Game => ServerGame;

		public ServerCardRepository CardRepository = new();

		//TODO networker, TODO awaiter, TODO notifier


		public override void _Ready()
		{
			base._Ready();
			ServerGame = new ServerGame(this, CardRepository);
		}

		public override void _Process(double delta)
		{
			base._Process(delta);
			networker?.Tick();
		}
	}
}