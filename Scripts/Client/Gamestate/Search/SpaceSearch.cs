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

		public event EventHandler? SearchFinished;

		public SpaceSearch(IEnumerable<Space> validSpaces, ClientNotifier clientNotifier)
		{
			this.clientNotifier = clientNotifier;
			this.validSpaces = new HashSet<Space>(validSpaces);
		}

		public bool IsCurrentTarget(GameCard card) => false;
		public bool IsValidTarget(GameCard card) => false;

		public void Select(GameCard card)
		{
			if (card.Position?.IsValid ?? false) Select(card.Position);
		}

		public void Select(Space space)
		{
			if (!validSpaces.Contains(space))
			{
				Logger.Log($"{space} is not a valid choice.");
				return;
			}

			clientNotifier.RequestSpaceTarget(space.x, space.y);
			SearchFinished?.Invoke(this, EventArgs.Empty);
		}

		public bool IsValidTarget(Space space) => validSpaces.Contains(space);

		public bool IsCurrentTarget(Space space) => false;
    }
}