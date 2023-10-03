using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientDiscard : Discard
	{
		public ClientDiscard(IPlayer owner) : base(owner) { }
	}
}