using System;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;
using Kompas.Server.Cards.Models;

namespace Kompas.Server.Cards.Controllers
{
	public class ServerCardController : ICardController
	{
		//FUTURE: when I want to display the card server side, have it store the card
		public Node3D Node => throw new NullReferenceException("Server cards don't have nodes yet");
		public IGameCardInfo Card => throw new NullReferenceException("Server cards don't have nodes yet");

		public event EventHandler? Refreshed;
		public event EventHandler<GameCard?>? AnythingRefreshed;
		public event EventHandler<GameCard?>? StatsRefreshed;
		public event EventHandler<GameCard?>? LinksRefreshed;
		public event EventHandler<GameCard?>? AugmentsRefreshed;
		public event EventHandler<GameCard?>? TargetingRefreshed;

		public void Delete() { }

		public void RefreshAugments()
		{
			AnythingRefreshed?.Invoke(this, null);
			AugmentsRefreshed?.Invoke(this, null);
		}
		public void RefreshLinks()
		{
			AnythingRefreshed?.Invoke(this, null);
			LinksRefreshed?.Invoke(this, null);
		}
		public void RefreshStats()
		{
			AnythingRefreshed?.Invoke(this, null);
			StatsRefreshed?.Invoke(this, null);
		}
		public void RefreshTargeting()
		{
			AnythingRefreshed?.Invoke(this, null);
			TargetingRefreshed?.Invoke(this, null);
		}
	}
}