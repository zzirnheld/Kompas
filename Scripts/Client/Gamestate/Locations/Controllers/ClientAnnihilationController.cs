using Kompas.Gamestate.Locations.Controllers;

namespace Kompas.Client.Gamestate.Locations.Controllers
{
	public partial class ClientAnnihilationController : AnnihilationController
	{
		public override void Refresh()
		{
			foreach (var card in AnnihilationModel.Cards)
			{
				card.NormalCardController.Node.Visible = false; //TODO spread them out somewhere
			}
		}

	}
}