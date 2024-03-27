using System;
using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;

namespace Kompas.Client.Gamestate.Search
{
	public interface ISearch
	{
		public event EventHandler? SearchFinished;

		public IReadOnlyCollection<(Location location, bool friendly)> SearchedLocations { get; }

		public void Select(GameCard card);
		public void Select(Space space);

		public bool IsValidTarget(GameCard card);
		public bool IsCurrentTarget(GameCard card);

		public bool IsValidTarget(Space space);
		public bool IsCurrentTarget(Space space);
	}
}