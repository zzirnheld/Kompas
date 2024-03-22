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
		public Location Location => Location.Board;

		protected readonly GameCard?[,] board = new GameCard[Space.BoardLen, Space.BoardLen];
		public IEnumerable<GameCard> Cards { get { foreach (var card in board) if (card != null) yield return card; } }

		private readonly BoardController boardController;

		protected Board(BoardController boardController)
		{
			this.boardController = boardController;
		}

		//helper methods
		#region helper methods
		public int IndexOf(GameCard card)
		{
			if (card.Location != Location.Board) return int.MinValue;
			_ = card.Position ?? throw new System.NullReferenceException("Card in play doesn't have a space!");
			return card.Position.Index;
		}

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
		public bool ValidSpellSpaceFor(GameCard? card, Space space)
		{
			//true for non-spells
			if (card == null || card.CardType != 'S') return true;

			var friendlyAvatar = card.ControllingPlayer.Avatar;
			var enemyAvatar = card.ControllingPlayer.Enemy.Avatar;
			_ = friendlyAvatar.Position ?? throw new System.NullReferenceException("Avatar didn't have a position!?");
			_ = enemyAvatar.Position ?? throw new System.NullReferenceException("Avatar didn't have a position!?");
			int dist = Space.DistanceBetween(friendlyAvatar.Position, enemyAvatar.Position, s => s != space && IsSpaceEmptyOfSpells(s));

			//if it's not in a relevant location, everything is fine
			return dist < Space.NoPathExists;
		}

		public bool Surrounded(Space s) => s.AdjacentSpaces.All(s => !IsEmpty(s));

		//get game data
		public bool IsEmpty(Space? s) => s != null && s.IsValid && GetCardAt(s) == null;

		public GameCard? GetCardAt(Space? s)
		{
			if (s == null) return null;
			if (!s.IsValid) return null;

			var (x, y) = s;
			return board[x, y];
		}

		public List<GameCard> CardsAdjacentTo(Space? space)
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
			foreach (var card in Cards) if (predicate(card)) list.Add(card);
			return list;
		}

		public List<GameCard> CardsAndAugsWhere(Predicate<GameCard> predicate)
		{
			var list = new List<GameCard>();
			foreach (var card in Cards)
			{
				if (predicate(card)) list.Add(card);
				if (card != null) list.AddRange(card.Augments.Where(c => predicate(c)));
			}
			return list;
		}

		public bool AreConnectedBy(Space source, Space destination, IRestriction<IGameCardInfo> restriction, IResolutionContext context)
			=> AreConnectedBy(source, destination, c => restriction.IsValid(c, context));

		public static bool AreConnectedBy(Space source, Space destination, IRestriction<Space> restriction, IResolutionContext context)
			=> Space.AreConnectedBy(source, destination, s => restriction.IsValid(s, context));

		public bool AreConnectedBy(Space source, Space destination, Func<GameCard?, bool> throughPredicate)
			=> Space.AreConnectedBy(source, destination, s => throughPredicate(GetCardAt(s)));

		public int EmptyDistanceBetween(GameCard src, Space dest)
			=> EmptyDistanceBetween(src.Position, dest);

		public int EmptyDistanceBetween(Space? src, Space dest)
			=> board[dest.x, dest.y] == null ? Space.DistanceBetween(src, dest, IsEmpty) : Space.NoPathExists;

		public int DistanceBetween(GameCard src, Space space, IRestriction<IGameCardInfo> restriction, IResolutionContext context)
			=> DistanceBetween(src.Position, space, c => restriction.IsValid(c, context));

		public int DistanceBetween(Space? src, Space? dest, Predicate<IGameCardInfo?> throughPredicate)
			=> Space.DistanceBetween(src, dest, s => throughPredicate(GetCardAt(s)));
		#endregion

		#region game mechanics
		public void Remove(GameCard toRemove)
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
		public virtual void Play(GameCard toPlay, Space to, IPlayer player, IStackable? stackSrc = null)
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
				var augmented = GetCardAt(to)
					?? throw new NullCardException($"Can't play an augment to empty space at {to}");
				//assuming there is a card there, try and add the augment. if it don't work, it borked.
				augmented.AddAugment(toPlay, stackSrc);

				toPlay.ControllingPlayer = player;
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

				toPlay.ControllingPlayer = player;

				boardController.Place(toPlay.CardController);
			}
		}

		//movement
		protected virtual void Swap(GameCard card, Space to, bool normal, IPlayer? mover, IStackable? stackSrc = null)
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
			GameCard? temp = board[toX, toY];
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

		public void Move(GameCard card, Space to, bool normal, IPlayer? mover, IStackable? stackSrc = null)
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