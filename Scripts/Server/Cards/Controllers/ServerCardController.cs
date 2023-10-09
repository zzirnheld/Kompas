using Kompas.Cards.Controllers;
using Kompas.Gamestate.Locations;
using Kompas.Server.Cards.Models;

namespace Kompas.Server.Cards.Controllers
{
	//[RequireComponent(typeof(ServerGameCard))]
	public class ServerCardController : ICardController
	{
		private readonly ServerGameCard serverCard;

		public ServerCardController(ServerGameCard serverCard)
		{
			this.serverCard = serverCard;
		}

		public void Delete() { }
		public void RefreshLinks() { }
		public void RefreshStats() { }
		public void SetPhysicalLocation(Location location) { }
	}
}