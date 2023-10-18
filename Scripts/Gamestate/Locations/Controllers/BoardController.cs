using Godot;
using Kompas.Cards.Controllers;

namespace Kompas.Gamestate.Locations.Controllers
{
	//Note: it may be more accurate to call this a view, and/or split it into view and controller
	public abstract partial class BoardController : Node
	{
		public abstract void Place(ICardController card);
	}
}