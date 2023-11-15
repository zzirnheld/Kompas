using Godot;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Gamestate.Locations.Controllers
{
	public abstract partial class HandController : Node
	{
		public Hand? HandModel { get; set; }

		public void Refresh() => SpreadAllCards();

		protected abstract void SpreadAllCards();
	}
}