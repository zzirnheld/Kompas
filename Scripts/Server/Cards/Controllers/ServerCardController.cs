using Kompas.Cards.Controllers;
using Kompas.Gamestate.Locations;
using Kompas.Server.Cards.Models;

namespace Kompas.Server.Cards.Controllers
{
	public class ServerCardController : ICardController
	{
		//FUTURE: when I want to display the card server side, have it store the card

		public void Delete() { }
		public void RefreshLinks() { }
		public void RefreshStats() { }
		public void SetPhysicalLocation(Location location) { }
	}
}