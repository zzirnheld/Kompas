using Godot;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Gamestate.Locations.Controllers
{
	public abstract partial class DeckController : Node //TODO shared parent class for location controllers? similar to models?
	{
		//TODO deck and discard should have logic for spreading out and collapsing cards. for now, just gonna splat them out
		public Deck? DeckModel { get; set; }

		public void Refresh() => SpreadOut();

		protected abstract void SpreadOut();
	}
}