using System;
using Kompas.Cards.Models;
using Kompas.Gamestate;

namespace Kompas.Client.Gamestate.Search
{
	public interface ISearch
	{
		public event EventHandler FinishSearch;

		public void Select(GameCard card);
		public void Select(Space space);

		public bool IsValidTarget(GameCard card);
		public bool IsCurrentTarget(GameCard card);
	}
}