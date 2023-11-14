using System;
using System.Collections.Generic;
using Godot;
using Kompas.Cards.Models;
using Kompas.Client.Networking;
using Kompas.Gamestate;

namespace Kompas.Client.Gamestate.Search
{
	public class SpaceSearch : ISearch
	{
		private readonly IReadOnlySet<Space> validSpaces;
		private readonly ClientNotifier clientNotifier;

		public SpaceSearch(IEnumerable<Space> validSpaces, ClientNotifier clientNotifier)
		{
			this.clientNotifier = clientNotifier;
			this.validSpaces = new HashSet<Space>(validSpaces);
		}

		public event EventHandler FinishSearch;

		public bool IsCurrentTarget(GameCard card) => false;
		public bool IsValidTarget(GameCard card) => false;

		public void Select(GameCard card)
		{
			if (card.Position.SafeIsValid()) Select(card.Position);
		}

		public void Select(Space space)
		{
			if (!validSpaces.Contains(space))
			{
				GD.Print($"{space} is not a valid choice.");
				return;
			}

			clientNotifier.RequestSpaceTarget(space.x, space.y);
			FinishSearch?.Invoke(this, EventArgs.Empty);
		}
	}
}