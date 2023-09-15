using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models
{
	public abstract class Board : ILocationModel
	{
		public const int SpacesInGrid = 7;
		public const int NoPathExists = 50;

		public Location Location => Location.Board;
		public abstract Game Game { get; }

		protected readonly GameCard[,] board = new GameCard[SpacesInGrid, SpacesInGrid];
		public IEnumerable<GameCard> Cards { get { foreach (var card in board) yield return card; } }

		public BoardController boardUIController;
		public void Refresh() => boardUIController.Refresh();

		//helper methods
		#region helper methods
		public int IndexOf(GameCard card) => card.Position.Index;

		private bool IsSpaceEmptyOfSpells(Space space)
		{
			var cardThere = GetCardAt(space);
			return cardThere == null || cardThere.CardType != 'S';
		}

		/// <summary>
		/// Checks whether there's too many spells already next to an Avatar
		/// </summary>
		/// <param name="card">The card to be checking whether it can go there</param>
		/// <param name="x">The x coordinate to check for</param>
		/// <param name="y">The y coordinate to check for</param>
		/// <returns><see langword="false"/> if the card is a spell, 
		/// <paramref name="x"/> and <paramref name="y"/> are next to an Avatar, 
		/// and there's already another spell next to that Avatar. <br></br> 
		/// <see langword="true"/> otherwise.</returns>
		public bool ValidSpellSpaceFor(GameCard card, Space space)
		{
			//true for non-spells
			if (card == null || card.CardType != 'S') return true;

			int dist = ShortestPath(card.ControllingPlayer.Avatar.Position, card.ControllingPlayer.Enemy.Avatar.Position, s => s != space && IsSpaceEmptyOfSpells(s));

			//if it's not in a relevant location, everything is fine
			return dist < NoPathExists;
		}

		public bool Surrounded(Space s) => s.AdjacentSpaces.All(s => !IsEmpty(s));

		//get game data
		public bool IsEmpty(Space s) => s.IsValid && GetCardAt(s) == null;

		public GameCard GetCardAt(Space s)
		{
			if (s.IsValid)
			{
				var (x, y) = s;
				return board[x, y];
			}
			else return null;
		}

		public List<GameCard> CardsAdjacentTo(Space space)
		{
			var list = new List<GameCard>();
			if (space == null)
			{
				//GD.PrintErr("Asking for cards adjacent to a null space");
				return list;
			}

			foreach (var s in space.AdjacentSpaces)
			{
				var card = GetCardAt(s);
				if (card != null) list.Add(card);
			}

			return list;
		}

		public List<GameCard> CardsWhere(Predicate<GameCard> predicate)
		{
			var list = new List<GameCard>();
			foreach (var card in board) if (predicate(card)) list.Add(card);
			return list;
		}

		public List<GameCard> CardsAndAugsWhere(Predicate<GameCard> predicate)
		{
			var list = new List<GameCard>();
			foreach (var card in board)
			{
				if (predicate(card)) list.Add(card);
				if (card != null) list.AddRange(card.Augments.Where(c => predicate(c)));
			}
			return list;
		}

		public bool AreConnectedBySpaces(Space source, Space destination, IRestriction<GameCardBase> restriction, IResolutionContext context)
			=> AreConnectedBySpaces(source, destination, c => restriction.IsValid(c, context));

		public bool AreConnectedBySpaces(Space source, Space destination, Func<GameCard, bool> throughPredicate)
			=> AreConnectedBySpaces(source, destination, s => throughPredicate(GetCardAt(s)));

		public bool AreConnectedBySpaces(Space source, Space destination, IRestriction<Space> restriction, IResolutionContext context)
			=> AreConnectedBySpaces(source, destination, s => restriction.IsValid(s, context));

		public bool AreConnectedBySpaces(Space source, Space destination, Func<Space, bool> predicate)
			=> destination.AdjacentSpaces.Any(destAdj => ShortestPath(source, destAdj, predicate) < NoPathExists);

		public bool AreConnectedByNumberOfSpacesFittingPredicate
			(Space source, Space destination, Func<Space, bool> spacePredicate, Func<int, bool> distancePredicate)
			=> destination.AdjacentSpaces.Any(destAdj => distancePredicate(ShortestPath(source, destAdj, spacePredicate)));

		public int ShortestEmptyPath(GameCard src, Space dest)
			=> ShortestEmptyPath(src.Position, dest);

		public int ShortestEmptyPath(Space src, Space dest)
			=> board[dest.x, dest.y] == null ? ShortestPath(src, dest, IsEmpty) : NoPathExists;

		public int ShortestPath(GameCard src, Space space, IRestriction<GameCardBase> restriction, IResolutionContext context)
			=> ShortestPath(src.Position, space, c => restriction.IsValid(c, context));

		public int ShortestPath(Space src, Space dest, Predicate<GameCard> throughPredicate)
			=> ShortestPath(src, dest, s => throughPredicate(GetCardAt(s)));

		/// <summary>
		/// A really bad Dijkstra's because this is a fun side project and I'm not feeling smart today
		/// </summary>
		/// <param name="start">The card to start looking from</param>
		/// <param name="x">The x coordinate you want a distance to</param>
		/// <param name="y">The y coordinate you want a distance to</param>
		/// <param name="throughPredicate">What all cards you go through must fit</param>
		/// <returns></returns>
		public int ShortestPath(Space start, Space destination, Func<Space, bool> throughPredicate)
		{
			if (start == destination) return 0;
			if (start == null || destination == null) return 50;

			int[,] dist = new int[7, 7];
			bool[,] seen = new bool[7, 7];

			var queue = new Queue<Space>();

			queue.Enqueue(start);
			dist[start.x, start.y] = 0;
			seen[start.x, start.y] = true;

			//set up the structures with the source node
			queue.Enqueue(start);

			//iterate until the queue is empty, in which case you'll have seen all connected cards that fit the restriction.
			while (queue.Any())
			{
				//consider the adjacent cards to the next node in the queue
				var curr = queue.Dequeue();
				var (currX, currY) = curr;
				foreach (var next in curr.AdjacentSpaces.Where(throughPredicate))
				{
					var (nextX, nextY) = next;
					//if that adjacent card is never seen before, initialize its distance and add it to the structures
					if (!seen[nextX, nextY])
					{
						seen[nextX, nextY] = true;
						queue.Enqueue(next);
						dist[nextX, nextY] = dist[currX, currY] + 1;
					}
					//otherwise, relax its distance if appropriate
					else if (dist[currX, currY] + 1 < dist[nextX, nextY])
						dist[nextX, nextY] = dist[currX, currY] + 1;
				}
			}

			return dist[destination.x, destination.y] <= 0 ? 50 : dist[destination.x, destination.y];
		}

		public bool ExistsCardOnBoard(Func<GameCard, bool> predicate)
		{
			foreach (var c in board)
			{
				if (predicate(c)) return true;
			}
			return false;
		}
		#endregion

		#region game mechanics
		public void Remove(GameCard toRemove)
		{
			if (toRemove.Location != Location.Board)
				throw new CardNotHereException(Location, toRemove, $"Tried to remove {toRemove} not on board");
			if (toRemove.Position == null)
				throw new InvalidSpaceException(toRemove.Position, "Can't remove a card from a null space");

			var (x, y) = toRemove.Position;
			if (board[x, y] == toRemove)
				board[x, y] = null;
			else
				throw new CardNotHereException(Location, toRemove, $"Card thinks it's at {toRemove.Position}, but {board[x, y]} is there");
		}

		/// <summary>
		/// Puts the card on the board
		/// </summary>
		/// <param name="toPlay">Card to be played</param>
		/// <param name="toX">X coordinate to play the card to</param>
		/// <param name="toY">Y coordinate to play the card to</param>
		public virtual void Play(GameCard toPlay, Space to, Player controller, IStackable stackSrc = null)
		{
			if (toPlay == null)
				throw new NullCardException($"Null card to play to {to}");
			if (toPlay.Location == Location.Board)
				throw new AlreadyHereException(Location, $"Tried to play {toPlay} to {to} even though it was already on the board at {toPlay?.Position}");
			if (to == null)
				throw new InvalidSpaceException(to, $"Space to play a card to cannot be null!");
			if (!ValidSpellSpaceFor(toPlay, to))
				throw new InvalidSpaceException(to, $"Tried to play {toPlay} to space {to}. This isn't ok, that's an invalid spell spot.");

			GD.Print($"In boardctrl, playing {toPlay.CardName} currently in {toPlay.Location} to {to}");

			//augments can't be played to a regular space.
			if (toPlay.CardType == 'A')
			{
				//augments therefore just get put on whatever card is on that space rn.
				var augmented = GetCardAt(to);
				//if there isn't a card, well, you can't do that.
				if (augmented == null) throw new NullCardException($"Can't play an augment to empty space at {to}");
				//assuming there is a card there, try and add the augment. if it don't work, it borked.
				augmented.AddAugment(toPlay, stackSrc);

				toPlay.ControllingPlayer = controller;
			}
			//otherwise, put a card to the requested space
			else
			{
				if (!IsEmpty(to)) throw new AlreadyHereException(Location, "There's already a card in a space to be played to");
				toPlay.Remove(stackSrc);
				var (toX, toY) = to;
				board[toX, toY] = toPlay;
				toPlay.Position = to;
				toPlay.LocationModel = this;

				toPlay.ControllingPlayer = controller;
			}
		}

		//movement
		protected virtual void Swap(GameCard card, Space to, bool playerInitiated, IStackable stackSrc = null)
		{
			GD.Print($"Swapping {card?.CardName} to {to}");

			if (!to.IsValid)
				throw new InvalidSpaceException(to);
			if (card == null)
				throw new NullCardException("Card to be swapped must not be null");
			if (card.Attached)
				throw new NotImplementedException();
			if (card.Location != Location.Board || card != GetCardAt(card.Position))
				throw new CardNotHereException(Location.Board, card,
					$"{card} not at {card.Position}, {GetCardAt(card.Position)} is there instead");

			var (tempX, tempY) = card.Position;
			var from = card.Position;
			var (toX, toY) = to;
			GameCard temp = board[toX, toY];
			//check valid spell positioning
			string swapDesc = $"Tried to move {card} to space {toX}, {toY}. " +
					$"{(temp == null ? "" : $"This would swap {temp.CardName} to {tempX}, {tempY}.")}";
			if (!ValidSpellSpaceFor(card, to)) throw new InvalidSpaceException(to, $"{swapDesc}, but the destination is an invalid spell space");
			if (!ValidSpellSpaceFor(temp, from)) throw new InvalidSpaceException(from, $"{swapDesc}, but the start is an invalid spell space");

			//then let the cards know they've been moved, but before moving them, so you can count properly
			if (playerInitiated)
			{
				card.CountSpacesMovedTo((toX, toY));
				temp?.CountSpacesMovedTo((tempX, tempY));
			}

			board[toX, toY] = card;
			board[tempX, tempY] = temp;

			card.Position = to;
			if (temp != null) temp.Position = from;
		}

		public void Move(GameCard card, Space to, bool playerInitiated, IStackable stackSrc = null)
		{
			if (card.Attached)
			{
				if (!to.IsValid)
					throw new InvalidSpaceException(to, $"Can't move {card} to invalid space");
				if (IsEmpty(to))
					throw new NullCardException($"Null card to attach {card} to at {to}");

				card.Remove(stackSrc);
				var (toX, toY) = to;
				board[toX, toY].AddAugment(card, stackSrc);
			}
			else Swap(card, to, playerInitiated, stackSrc);
		}
		#endregion game mechanics

		public override string ToString()
		{
			var sb = new StringBuilder();
			for (int i = 0; i < 7; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					var card = board[i, j];
					if (card != null) sb.Append($"At {i}, {j}, {card.CardName} id {card.ID}");
				}
			}
			return sb.ToString();
		}
	}
}