using System;
using Godot;
using Kompas.Cards.Models;

namespace Kompas.Cards.Controllers
{
	public interface ICardController
	{
		public event EventHandler Refreshed;

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

		public void RefreshTargeting();

		public void Delete();

		public Node3D Node { get; }
		public IGameCardInfo Card { get; }

	}
}