using System;
using Godot;
using Kompas.Cards.Models;

namespace Kompas.Cards.Controllers
{
	public interface ICardController
	{
		/// <summary>
		/// Anything on the card has been refreshed
		/// </summary>
		public event EventHandler<GameCard?>? AnythingRefreshed;

		/// <summary>
		/// Refreshes the stats displayed for this card.
		/// Should refresh anything showing this card: the model for this card, the mouse-over UI, etc.
		/// </summary>
		public void RefreshStats();
		public event EventHandler<GameCard?>? StatsRefreshed;

		/// <summary>
		/// Refreshes showing cards linked to this card.
		/// </summary>
		public void RefreshLinks();
		public event EventHandler<GameCard?>? LinksRefreshed;
		
		public void RefreshAugments();
		public event EventHandler<GameCard?>? AugmentsRefreshed;

		public void RefreshTargeting();
		public event EventHandler<GameCard?>? TargetingRefreshed;

		public void Delete();

		public Node3D Node { get; }
		public IGameCardInfo Card { get; }

	}
}