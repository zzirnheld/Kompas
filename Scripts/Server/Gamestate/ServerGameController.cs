using Godot;
using Kompas.Gamestate;
using System;

namespace Kompas.Server.Gamestate
{
	public partial class ServerGameController : GameController
	{
		public ServerGame ServerGame { get; private set; }
		public override IGame Game => ServerGame;
	}
}