using Godot;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Gamestate.Locations.Controllers
{
	public abstract partial class DiscardController : Node //TODO shared parent class for location controllers? similar to models?
	{
		public Discard DiscardModel { get; set; }

		public void Refresh() => SpreadOut();

		protected abstract void SpreadOut();
	}
}