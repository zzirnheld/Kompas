using Kompas.Client.Cards.Models;
using Kompas.Client.Gamestate.Players;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientBoard : Board
	{
		private readonly ClientGame game;
		public override Game Game => game;

		public ClientBoard(ClientGame game)
		{
			this.game = game;
		}
	}
}