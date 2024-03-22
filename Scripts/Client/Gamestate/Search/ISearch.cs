using System;
using Kompas.Cards.Models;
using Kompas.Gamestate;

namespace Kompas.Client.Gamestate.Search
{
	public interface ISearch
	{
		public event EventHandler SearchFinished;

		public void Select(IGameCard card);
		public void Select(Space space);

		public bool IsValidTarget(IGameCard card);
		public bool IsCurrentTarget(IGameCard card);
	}
}