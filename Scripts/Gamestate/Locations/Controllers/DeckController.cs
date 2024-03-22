using Godot;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Models;
using Kompas.Shared.Exceptions;

namespace Kompas.Gamestate.Locations.Controllers
{
	public abstract partial class DeckController : Node //TODO shared parent class for location controllers? similar to models?
	{
		private IDeck? _deckModel;
		public IDeck DeckModel
		{
			get => _deckModel ?? throw new UnassignedReferenceException();
			set => _deckModel = value;
		}

		public void Refresh() => SpreadOut();

		protected abstract void SpreadOut();
	}
}