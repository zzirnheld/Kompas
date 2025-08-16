using Kompas.Cards.Controllers;
using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Server.Gamestate.Locations.Controllers;

public partial class ServerDiscardController : DiscardController
{
	public override void RefreshJustAdded(ICardController cardController) { }

	protected override void SpreadOut() { }
}