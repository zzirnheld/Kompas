using Godot;
using Kompas.Cards.Controllers;

namespace Kompas.Gamestate.Locations.Controllers
{
	//Note: it may be more accurate to call this a view, and/or split it into view and controller
	public abstract partial class BoardController : Node
	{
		//TODO: replace with a "move" and a "play" function, to eventually add animations distinct to each
		public abstract void Place(ICardController card);
	}
}