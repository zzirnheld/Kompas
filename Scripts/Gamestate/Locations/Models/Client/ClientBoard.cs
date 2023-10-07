using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientBoard : Board
	{
		public ClientBoard(BoardController boardController) : base(boardController) { }
	}
}