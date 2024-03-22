using System;
using System.Linq;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Cards.Models
{
	public static class GameCardExtensions
	{
		public static void SwapCharStats(this IGameCard source, IGameCard other, bool swapN = true, bool swapE = true, bool swapS = true, bool swapW = true)
		{
			int[] aNewStats = new int[4];
			int[] bNewStats = new int[4];

			(aNewStats[0], bNewStats[0]) = swapN ? (other.N, source.N) : (source.N, other.N);
			(aNewStats[1], bNewStats[1]) = swapE ? (other.E, source.E) : (source.E, other.E);
			(aNewStats[2], bNewStats[2]) = swapS ? (other.S, source.S) : (source.S, other.S);
			(aNewStats[3], bNewStats[3]) = swapW ? (other.W, source.W) : (source.W, other.W);

			source.SetCharStats(aNewStats[0], aNewStats[1], aNewStats[2], aNewStats[3]);
			other.SetCharStats(bNewStats[0], bNewStats[1], bNewStats[2], bNewStats[3]);
		}

		public static bool HasSubtype(this IGameCardInfo card, string subtype) => card.SubtypeText.ToLower().Contains(subtype.ToLower());

		public static int RadiusDistanceTo(this IGameCardInfo card, Space space)
		{
			if (card.Location != Location.Board) return int.MaxValue;
			_ = card.Position ?? throw new System.NullReferenceException("Can't get the radial distance to a card in play with a null space!");
			return card.Position.RadiusDistanceTo(space);
		}
			
		public static int DistanceTo(this IGameCardInfo card, Space space)
		{
			if (card.Location != Location.Board) return int.MaxValue;
			_ = card.Position ?? throw new System.NullReferenceException("Can't get the distance to a card in play with a null space!");
			return card.Position.DistanceTo(space);
		}

		public static int DistanceTo(this IGameCardInfo card, IGameCardInfo other)
		{
			if (other.Location != Location.Board) return int.MaxValue;
			_ = other.Position ?? throw new System.NullReferenceException("Can't get the distance to a a card in play's null space!");
			return card.DistanceTo(other.Position);
		}

		public static bool WithinSpaces(this IGameCardInfo card, int numSpaces, IGameCardInfo other)
			=> card.DistanceTo(other) <= numSpaces;

		public static bool IsAdjacentTo(this IGameCardInfo card, IGameCardInfo other)
		{
			if (other?.Location != Location.Board) return false;
			_ = other.Position ?? throw new System.NullReferenceException("Another card in play with a null space can't be adjacent to anything!");
			return card.IsAdjacentTo(other.Position);
		}

		public static bool IsAdjacentTo(this IGameCardInfo card, Space space)
		{
			if (card.Location != Location.Board) return false;
			_ = card.Position ?? throw new System.NullReferenceException("A card in play with a null space can't be adjacent to anything!");
			return card.Position.IsAdjacentTo(space);
		}

		public static bool IsAdjacentTo(this IGameCardInfo card, Predicate<IGameCardInfo> predicate)
		{
			if (card.Location != Location.Board) return false;
			_ = card.Position ?? throw new System.NullReferenceException("A card in play with a null space can't be adjacent to anything!");
			return card.Game.Board.CardsAdjacentTo(card.Position).Any(c => predicate(c));
		}

		/// <summary>
		/// Whether <paramref name="space"/> is in this card's AOE if this card is at <paramref name="mySpace"/>
		/// </summary>
		public static bool SpaceInAOE(this IGameCardInfo card, Space? space, Space? mySpace)
			=> space != null
			&& mySpace != null
			&& card.SpellSubtypes != null
			&& card.SpellSubtypes.Any(s => s switch
			{
				CardBase.RadialSubtype => mySpace.DistanceTo(space) <= card.Radius,
				_ => false
			});

		public static bool SpaceInAOE(this IGameCardInfo card, Space space) => card.SpaceInAOE(space, card.Position);

		/// <summary>
		/// Whether <paramref name="other"/> is in this card's AOE if this card is at <paramref name="mySpace"/>
		/// </summary>
		public static bool CardInAOE(this IGameCardInfo card, IGameCardInfo other, Space mySpace) => card.SpaceInAOE(other.Position, mySpace);

		/// <summary>
		/// Whether <paramref name="other"/> is in the aoe of <see cref="this"/> card.
		/// </summary>
		public static bool CardInAOE(this IGameCardInfo card, IGameCardInfo other)
		{
			if (card.Location != Location.Board) return false;
			_ = card.Position ?? throw new System.NullReferenceException("A card in play with a null space can't contain anything!");
			return card.CardInAOE(other, card.Position);
		}

		/// <summary>
		/// Whether <paramref name="other"/> and this card have any spaces shared between their AOEs,
		/// if this card is at <paramref name="mySpace"/>
		/// </summary>
		public static bool Overlaps(this IGameCardInfo card, IGameCardInfo other, Space mySpace)
			=> Space.Spaces.Any(sameSpace => card.SpaceInAOE(sameSpace, mySpace)
										  && other.SpaceInAOE(sameSpace));

		/// <summary>
		/// Whether <paramref name="c"/> and this card have any spaces shared between their AOEs
		/// </summary>
		public static bool Overlaps(this IGameCardInfo card, IGameCardInfo c)
		{
			if (card.Location != Location.Board) return false;
			_ = card.Position ?? throw new System.NullReferenceException("A card in play with a null space can't overlap anything!");
			return card.Overlaps(c, card.Position);
		}

		public static bool SameColumn(this IGameCardInfo card, Space space)
		{
			if (card.Location != Location.Board) return false;
			_ = card.Position ?? throw new System.NullReferenceException("A card in play with a null space can't be in the same column as anything!");
			return card.Position.SameColumn(space);
		}

		public static bool SameColumn(this IGameCardInfo card, IGameCardInfo other)
		{
			if (other.Location != Location.Board) return false;
			_ = other.Position ?? throw new System.NullReferenceException("Another card in play with a null space can't be in the same column as anything!");
			return card.SameColumn(other.Position);
		}

		/// <summary>
		/// Returns whether the <paramref name="space"/> passed in is in front of this card
		/// </summary>
		/// <param name="space">The space to check if it's in front of this card</param>
		/// <returns><see langword="true"/> if <paramref name="space"/> is in front of this, <see langword="false"/> otherwise.</returns>
		public static bool SpaceInFront(this IGameCardInfo card, Space space)
		{
			if (card.Location != Location.Board) return false;
			_ = card.Position ?? throw new System.NullReferenceException("A card in play with a null space can't be in front of anything!");
			_ = card.SubjectivePosition ?? throw new System.NullReferenceException("A card in play with a null space can't be in front of anything!");
			return card.ControllingPlayer.SubjectiveCoords(space).NorthOf(card.SubjectivePosition);
		}

		/// <summary>
		/// Returns whether the card passed in is in front of this card
		/// </summary>
		/// <param name="other">The card to check if it's in front of this one</param>
		/// <returns><see langword="true"/> if <paramref name="other"/> is in front of this, <see langword="false"/> otherwise.</returns>
		public static bool CardInFront(this IGameCardInfo card, IGameCardInfo? other)
		{
			if (other?.Location != Location.Board) return false;
			_ = other.Position ?? throw new System.NullReferenceException("Another card in play with a null space can't be in front of anything!");
			return card.SpaceInFront(other.Position);
		}

		/// <summary>
		/// Returns whether the <paramref name="space"/> passed in is behind this card
		/// </summary>
		/// <param name="space">The space to check if it's behind this card</param>
		/// <returns><see langword="true"/> if <paramref name="space"/> is behind this, <see langword="false"/> otherwise.</returns>
		public static bool SpaceBehind(this IGameCardInfo card, Space space)
		{
			if (card.Location != Location.Board) return false;
			_ = card.Position ?? throw new System.NullReferenceException("A card in play with a null space can't be in front of anything!");
			_ = card.SubjectivePosition ?? throw new System.NullReferenceException("A card in play with a null space can't be in front of anything!");
			return card.SubjectivePosition.NorthOf(card.ControllingPlayer.SubjectiveCoords(space));
		}

		/// <summary>
		/// Returns whether the card passed in is behind this card
		/// </summary>
		/// <param name="other">The card to check if it's behind this one</param>
		/// <returns><see langword="true"/> if <paramref name="other"/> is behind this, <see langword="false"/> otherwise.</returns>
		public static bool CardBehind(this IGameCardInfo card, IGameCardInfo? other)
		{
			if (other?.Location != Location.Board) return false;
			_ = other.Position ?? throw new System.NullReferenceException("Another card in play with a null space can't be behind of anything!");
			return card.SpaceBehind(other.Position);
		}

		public static bool SpaceDirectlyInFront(this IGameCardInfo card, Space space)
		{
			if (card.Location != Location.Board) return false;
			_ = card.SubjectivePosition ?? throw new System.NullReferenceException("A card in play with a null space can't be in front of anything!");
			return card.ControllingPlayer.SubjectiveCoords(space) == card.SubjectivePosition.DueNorth;
		}

		public static bool CardDirectlyInFront(this IGameCardInfo card, IGameCardInfo? other)
		{
			if (other?.Location != Location.Board) return false;
			_ = other.Position ?? throw new System.NullReferenceException("Another card in play with a null space can't be directly in front of anything!");
			return card.SpaceDirectlyInFront(other.Position);
		}

		public static bool SameDiagonal(this IGameCardInfo card, Space space)
		{
			if (card.Location != Location.Board) return false;
			_ = card.Position ?? throw new System.NullReferenceException("A card in play with a null space can't be in the same diagonal as anything!");
			return card.Position.SameDiagonal(space);
		}

		public static bool SameDiagonal(this IGameCardInfo card, IGameCardInfo? other)
		{
			if (other?.Location != Location.Board) return false;
			_ = other.Position ?? throw new System.NullReferenceException("Another card in play with a null space can't be in the same diagonal as anything!");
			return card.SameDiagonal(other.Position);
		}

		public static bool InCorner(this IGameCardInfo card)
		{
			if (card.Location != Location.Board) return false;
			_ = card.Position ?? throw new System.NullReferenceException("A card in play with a null space can't be in a corner!");
			return card.Position.IsCorner;
		}

		/// <summary>
		/// Refers to this situation: <br></br>
		/// | <paramref name="space"/> | <br></br>
		/// | this card | <br></br>
		/// | <paramref name="other"/> param | <br></br>
		/// </summary>
		/// <param name="space">The space in the same axis as this card and <paramref name="other"/> param</param>
		/// <param name="other">The card in the same axis as this card and the <paramref name="space"/> param.</param>
		/// <returns></returns>
		public static bool SpaceDirectlyAwayFrom(this IGameCardInfo card, (int x, int y) space, IGameCardInfo other)
		{
			if (other.Location != Location.Board || card.Location != Location.Board) return false;
			_ = card.Position ?? throw new System.NullReferenceException("A card in play with a null space can't be directly away from anything!");
			_ = other.Position ?? throw new System.NullReferenceException("A card in play with a null space can't be directly away from anything!");

			int xDiffCard = other.Position.x - card.Position.x;
			int yDiffCard = other.Position.y - card.Position.y;
			int xDiffSpace = space.x - card.Position.x;
			int yDiffSpace = space.y - card.Position.y;

			return (xDiffCard == 0 && xDiffSpace == 0)
				|| (yDiffCard == 0 && yDiffSpace == 0)
				|| (xDiffCard == yDiffCard && xDiffSpace == yDiffSpace);
		}

		public static int ShortestPath(this IGameCardInfo card, Space space, Predicate<IGameCardInfo?> throughPredicate)
		{
			if (card.Location != Location.Board) return Space.NoPathExists;
			_ = card.Position ?? throw new System.NullReferenceException("A card in play with a null space can't get a shortest path to anything!");
			return card.Game.Board.ShortestPath(card.Position, space, throughPredicate);
		}
	}
}