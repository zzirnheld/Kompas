using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models.Client
{
	public class ClientDiscard : Discard
	{
		public ClientDiscard(IPlayer owner, DiscardController discardController) : base(owner, discardController)
		{
		}
	}
}