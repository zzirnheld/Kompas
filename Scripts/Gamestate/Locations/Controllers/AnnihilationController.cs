using Godot;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Gamestate.Locations.Controllers
{
	public partial class AnnihilationController : Node //TODO shared parent class for location controllers? similar to models?
	{
		private Annihilation? _annihilationModel;
		public Annihilation AnnihilationModel
		{
			get => _annihilationModel ?? throw new UnassignedReferenceException();
			set => _annihilationModel = value;
		}

		public void Refresh() { }
	}
}