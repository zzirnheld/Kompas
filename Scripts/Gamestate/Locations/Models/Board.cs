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
	public interface IBoard : ILocationModel
	{
		public IGameCard? GetCardAt(Space? space);
		public IEnumerable<IGameCard> CardsAdjacentTo(Space? space);
		public int ShortestPath(Space? from, Space? to, Predicate<IGameCardInfo?> throughPredicate);
	}

	public interface IBoard<CardType, PlayerType> : ILocationModel<CardType, PlayerType>, IBoard
		where CardType : class, IGameCard<CardType, PlayerType>
		where PlayerType : IPlayer<CardType, PlayerType>
	{
		public new CardType? GetCardAt(Space space);
		public new IEnumerable<CardType> CardsAdjacentTo(Space? space);

		public void Play(CardType card, Space to, PlayerType controller, IStackable? stackSrc = null);
		public void Move(CardType card, Space to, bool normal, PlayerType? mover, IStackable? stackSrc = null);
	}

	public static class BoardExensions
	{
		public static bool Surrounded(this IBoard board, Space s) => s.AdjacentSpaces.All(s => !board.IsEmpty(s));

		public static bool IsEmpty(this IBoard board, Space? s) => s != null && s.IsValid && board.GetCardAt(s) == null;

	}

	public abstract class Board<CardType, PlayerType> : IBoard<CardType, PlayerType>
		where CardType : class, IGameCard<CardType, PlayerType>
		where PlayerType : IPlayer<CardType, PlayerType>
	{
		public Location Location => Location.Board;

		protected readonly CardType?[,] board = new CardType[Space.BoardLen, Space.BoardLen];
		public IEnumerable<CardType> Cards { get { foreach (var card in board) if (card != null) yield return card; } }
		IEnumerable<IGameCard> ILocationModel.Cards => Cards;

		private readonly BoardController boardController;

		protected Board(BoardController boardController)
		{
			this.boardController = boardController;
		}

		//helper methods
		#region helper methods
		public int IndexOf(CardType card)
		{
			if (card.Location != Location.Board) return int.MinValue;
			_ = card.Position ?? throw new System.NullReferenceException("Card in play doesn't have a space!");
			return card.Position.Index;
		}

		private bool IsSpaceEmptyOfSpells(Space space)
		{
			var cardThere = GetCardAt(space);
			return cardThere == null || cardThere.Type != 'S';
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
		public bool ValidSpellSpaceFor(IGameCard? card, Space space)
		{
			//true for non-spells
			if (card == null || card.Type != 'S') return true;

			var friendlyAvatar = card.ControllingPlayer.Avatar;
			var enemyAvatar = card.ControllingPlayer.Enemy.Avatar;
			_ = friendlyAvatar.Position ?? throw new System.NullReferenceException("Avatar didn't have a position!?");
			_ = enemyAvatar.Position ?? throw new System.NullReferenceException("Avatar didn't have a position!?");
			int dist = ShortestPath(friendlyAvatar.Position, enemyAvatar.Position, s => s != space && IsSpaceEmptyOfSpells(s));

			//if it's not in a relevant location, everything is fine
			return dist < Space.NoPathExists;
		}

		public CardType? GetCardAt(Space? s)
		{
			if (s == null) return null;
			if (!s.IsValid) return null;

			var (x, y) = s;
			return board[x, y];
		}
		IGameCard? IBoard.GetCardAt(Space? space) => GetCardAt(space);

		public IEnumerable<CardType> CardsAdjacentTo(Space? space)
		{
			var list = new List<CardType>();
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
		IEnumerable<IGameCard> IBoard.CardsAdjacentTo(Space? space) => CardsAdjacentTo(space);

		public List<CardType> CardsWhere(Predicate<IGameCard> predicate)
		{
			var list = new List<CardType>();
			foreach (var card in Cards) if (predicate(card)) list.Add(card);
			return list;
		}

		public List<CardType> CardsAndAugsWhere(Predicate<IGameCard> predicate)
		{
			var list = new List<CardType>();
			foreach (var card in Cards)
			{
				if (predicate(card)) list.Add(card);
				if (card != null) list.AddRange(card.Augments.Where(c => predicate(c)));
			}
			return list;
		}

		public bool AreConnectedBySpaces(Space source, Space destination, IRestriction<IGameCardInfo> restriction, IResolutionContext context)
			=> AreConnectedBySpaces(source, destination, c => restriction.IsValid(c, context));

		public bool AreConnectedBySpaces(Space source, Space destination, Func<IGameCard?, bool> throughPredicate)
			=> AreConnectedBySpaces(source, destination, s => throughPredicate(GetCardAt(s)));

		public static bool AreConnectedBySpaces(Space source, Space destination, IRestriction<Space> restriction, IResolutionContext context)
			=> AreConnectedBySpaces(source, destination, s => restriction.IsValid(s, context));

		public static bool AreConnectedBySpaces(Space source, Space destination, Func<Space, bool> predicate)
			=> destination.AdjacentSpaces.Any(destAdj => ShortestPath(source, destAdj, predicate) < Space.NoPathExists);

		public static bool AreConnectedByNumberOfSpacesFittingPredicate
			(Space? source, Space destination, Func<Space, bool> spacePredicate, Func<int, bool> distancePredicate)
			=> destination.AdjacentSpaces.Any(destAdj => distancePredicate(ShortestPath(source, destAdj, spacePredicate)));

		public int ShortestEmptyPath(IGameCard src, Space dest)
			=> ShortestEmptyPath(src.Position, dest);

		public int ShortestEmptyPath(Space? src, Space dest)
			=> board[dest.x, dest.y] == null ? ShortestPath(src, dest, this.IsEmpty) : Space.NoPathExists;

		public int ShortestPath(IGameCard src, Space space, IRestriction<IGameCardInfo> restriction, IResolutionContext context)
			=> ShortestPath(src.Position, space, c => restriction.IsValid(c, context));

		public int ShortestPath(Space? src, Space? dest, Predicate<IGameCardInfo?> throughPredicate)
			=> ShortestPath(src, dest, s => throughPredicate(GetCardAt(s)));

		/// <summary>
		/// A really bad Dijkstra's because this is a fun side project and I'm not feeling smart today
		/// </summary>
		/// <param name="start">The card to start looking from</param>
		/// <param name="x">The x coordinate you want a distance to</param>
		/// <param name="y">The y coordinate you want a distance to</param>
		/// <param name="throughPredicate">What all cards you go through must fit</param>
		/// <returns></returns>
		public static int ShortestPath(Space? start, Space? destination, Func<Space, bool> throughPredicate)
		{
			if (start == destination) return 0;
			if (start == null || destination == null) return Space.NoPathExists;

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

			return dist[destination.x, destination.y] <= 0 ? Space.NoPathExists : dist[destination.x, destination.y];
		}
		#endregion

		#region game mechanics
		public void Remove(CardType toRemove)
		{
			if (toRemove.Location != Location.Board)
				throw new CardNotHereException(Location, toRemove, $"Tried to remove {toRemove} not on board");
			if (toRemove.Position == null)
				throw new InvalidSpaceException(toRemove.Position, "Can't remove a card from a null space");

			boardController.Remove(toRemove.CardController);
			
			var (x, y) = toRemove.Position;
			if (board[x, y] == toRemove)
				board[x, y] = null;
			else
				throw new CardNotHereException(Location, toRemove, $"Card thinks it's at {toRemove.Position}, but {board[x, y]} is there");
		}

		/// <summary>
		/// Puts the card on the board.
		/// </summary>
		/// <param name="toPlay">Card to be played</param>
		/// <param name="toX">X coordinate to play the card to</param>
		/// <param name="toY">Y coordinate to play the card to</param>
		public virtual void Play(CardType toPlay, Space to, PlayerType player, IStackable? stackSrc = null)
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
			if (toPlay.Type == 'A')
			{
				//augments therefore just get put on whatever card is on that space rn.
				var augmented = GetCardAt(to)
					?? throw new NullCardException($"Can't play an augment to empty space at {to}");
				//assuming there is a card there, try and add the augment. if it don't work, it borked.
				augmented.AddAugment(toPlay, stackSrc);

				TakeControl(toPlay, player);
			}
			//otherwise, put a card to the requested space
			else
			{
				if (!this.IsEmpty(to)) throw new AlreadyHereException(Location, "There's already a card in a space to be played to");
				toPlay.Remove(stackSrc);
				var (toX, toY) = to;
				board[toX, toY] = toPlay;
				toPlay.Position = to;
				toPlay.LocationModel = this;

				TakeControl(toPlay, player);

				boardController.Place(toPlay.CardController);
			}
		}

		protected abstract void TakeControl(CardType card, PlayerType player);

		//movement
		protected virtual void Swap(CardType card, Space to, bool normal, IPlayer? mover, IStackable? stackSrc = null)
		{
			GD.Print($"Swapping {card?.CardName} to {to}");

			if (!to.IsValid)
				throw new InvalidSpaceException(to);
			if (card == null)
				throw new NullCardException("Card to be swapped must not be null");
			if (card.AugmentedCard != null)
				throw new NotImplementedException();
			if (card.Location != Location.Board
				|| card.Position == null
				|| card != GetCardAt(card.Position))
				throw new CardNotHereException(Location.Board, card,
					$"{card} not at {card.Position}, {GetCardAt(card.Position)} is there instead");

			var (tempX, tempY) = card.Position;
			var from = card.Position;
			var (toX, toY) = to;
			var temp = board[toX, toY];
			//check valid spell positioning
			string swapDesc = $"Tried to move {card} to space {toX}, {toY}. " +
					$"{(temp == null ? "" : $"This would swap {temp.CardName} to {tempX}, {tempY}.")}";
			if (!ValidSpellSpaceFor(card, to)) throw new InvalidSpaceException(to, $"{swapDesc}, but the destination is an invalid spell space");
			if (!ValidSpellSpaceFor(temp, from)) throw new InvalidSpaceException(from, $"{swapDesc}, but the start is an invalid spell space");


			boardController.Remove(card.CardController);
			if (temp != null) boardController.Remove(temp.CardController);

			//then let the cards know they've been moved, but before moving them, so you can count properly
			if (normal)
			{
				card.CountSpacesMovedTo((toX, toY));
				temp?.CountSpacesMovedTo((tempX, tempY));
			}

			board[toX, toY] = card;
			board[tempX, tempY] = temp;

			card.Position = to;
			if (temp != null) temp.Position = from;

			boardController.Place(card.CardController);
			if (temp != null) boardController.Place(temp.CardController);
		}

		public void Move(CardType card, Space to, bool normal, PlayerType? mover, IStackable? stackSrc = null)
		{
			if (card.AugmentedCard != null)
			{
				if (!to.IsValid)
					throw new InvalidSpaceException(to, $"Can't move {card} to invalid space");

				var target = board[to.x, to.y] ?? throw new NullCardException($"Null card to attach {card} to at {to}");

				card.Remove(stackSrc);
				target.AddAugment(card, stackSrc);
			}
			else Swap(card, to, normal, mover, stackSrc);
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