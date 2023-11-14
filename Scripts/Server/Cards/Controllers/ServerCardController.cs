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

		public void Delete() { }

		public void RefreshAugments() { }
		public void RefreshLinks() { }
		public void RefreshStats() { }
		public void RefreshTargeting() { }
	}
}