using Godot;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Gamestate.Locations.Controllers
{
	public partial class AnnihilationController : Node //TODO shared parent class for location controllers? similar to models?
	{
		public Annihilation? AnnihilationModel { get; set; }

		public void Refresh() { }
	}
}