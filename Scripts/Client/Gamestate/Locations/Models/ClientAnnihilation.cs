using Kompas.Client.Cards.Models;
using Kompas.Client.Gamestate.Players;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Client.Gamestate.Locations.Models
{
	public class ClientAnnihilation : Annihilation<ClientGameCard, ClientPlayer>
	{
		public ClientAnnihilation(ClientPlayer owner, AnnihilationController annihilationController) : base(owner, annihilationController)
		{
		}

		public override void TakeControlOf(ClientGameCard card)
		{
			card.ControllingPlayer = Owner;
		}
	}
}