using Godot;
using Kompas.Gamestate.Locations;

namespace Kompas.Cards.Controllers
{
	public abstract partial class CardController : Node // TODO should expend a specific node
	{
		/// <summary>
        /// Refreshes the stats displayed for this card.
        /// Should refresh anything showing this card: the model for this card, the mouse-over UI, etc.
        /// </summary>
		public void RefreshStats()
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
        /// Refreshes showing cards linked to this card.
        /// </summary>
		public void RefreshLinks()
		{
			throw new System.NotImplementedException();
		}

		public void SetPhysicalLocation(Location location)
		{
			throw new System.NotImplementedException();
		}
	}
}