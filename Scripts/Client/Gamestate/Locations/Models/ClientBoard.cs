using Kompas.Client.Cards.Models;
using Kompas.Client.Gamestate.Players;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientBoard : Board<ClientGameCard, ClientPlayer>
	{
		public ClientBoard(BoardController boardController) : base(boardController) { }

		public void Play(ClientGameCard card, Space to, ClientPlayer controller) => base.Play(card, to, controller);

		protected override void TakeControl(ClientGameCard card, ClientPlayer player) => card.ControllingPlayer = player;
	}
}