using Godot;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;

namespace Kompas.Cards.Controllers
{
	public interface ICardController
	{
		/// <summary>
		/// Refreshes the stats displayed for this card.
		/// Should refresh anything showing this card: the model for this card, the mouse-over UI, etc.
		/// </summary>
		public void RefreshStats();

		/// <summary>
		/// Refreshes showing cards linked to this card.
		/// </summary>
		public void RefreshLinks();
		
		public void RefreshAugments();

		public void Delete();

		public Node3D Node { get; }
		public IGameCard Card { get; }

	}
}