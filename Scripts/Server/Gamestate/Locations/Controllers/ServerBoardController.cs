using Kompas.Cards.Controllers;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Server.Gamestate.Locations.Controllers;

public partial class ServerBoardController : BoardController
{
	public override void Move(ICardController card, MovePath path) { }

	public override void Play(ICardController card) { }
}