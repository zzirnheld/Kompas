using Kompas.Cards.Controllers;

namespace Kompas.Gamestate.Locations.Controllers;

public interface ILocationController
{
	public void Refresh();
	public void Remove(ICardController cardController);
	public void Refresh(ICardController justAdded);
}