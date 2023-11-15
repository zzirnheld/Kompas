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
		public Node3D Node => null;
		public IGameCardInfo Card => null;

		public event EventHandler Refreshed;
		public event EventHandler<GameCard> AnythingRefreshed;
		public event EventHandler<GameCard> StatsRefreshed;
		public event EventHandler<GameCard> LinksRefreshed;
		public event EventHandler<GameCard> AugmentsRefreshed;
		public event EventHandler<GameCard> TargetingRefreshed;

		public void Delete() { }

		public void RefreshAugments()
		{
			AnythingRefreshed?.Invoke(this, Card.Card);
			AugmentsRefreshed?.Invoke(this, Card.Card);
		}
		public void RefreshLinks()
		{
			AnythingRefreshed?.Invoke(this, Card.Card);
			LinksRefreshed?.Invoke(this, Card.Card);
		}
		public void RefreshStats()
		{
			AnythingRefreshed?.Invoke(this, Card.Card);
			StatsRefreshed?.Invoke(this, Card.Card);
		}
		public void RefreshTargeting()
		{
			AnythingRefreshed?.Invoke(this, Card.Card);
			TargetingRefreshed?.Invoke(this, Card.Card);
		}
	}
}