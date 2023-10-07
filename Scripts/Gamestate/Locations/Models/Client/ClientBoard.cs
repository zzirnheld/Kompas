using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Gamestate.Locations.Models.Client
{
	public class ClientBoard : Board
	{
		public ClientBoard(BoardController boardController) : base(boardController) { }
	}
}