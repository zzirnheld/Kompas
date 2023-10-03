using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;

namespace Kompas.Cards.Models
{
	public interface IGameCard
	{
		public GameCard Card { get; }
		public int IndexInList { get; }
		public Location Location { get; }
		public Space Position { get; }

		public string[] SpellSubtypes { get; }
		public int Radius { get; }
	}

	public static class GameCardExtensions
	{
		public static int RadiusDistanceTo(this IGameCard card, Space space)
			=> card.Location == Location.Board ? card.Position.RadiusDistanceTo(space) : int.MaxValue;
		public static int DistanceTo(this IGameCard card, Space space)
			=> card.Location == Location.Board ? card.Position.DistanceTo(space) : int.MaxValue;
		public static int DistanceTo(this IGameCard card, GameCardBase other) => card.DistanceTo(other.Position);

		public static bool WithinSpaces(this IGameCard card, int numSpaces, GameCardBase other)
			=> card.Location == Location.Board
			&& other?.Location == Location.Board
			&& card.DistanceTo(other) <= numSpaces;

		public static bool IsAdjacentTo(this IGameCard card, GameCardBase other)
			=> card.Location == Location.Board
			&& other?.Location == Location.Board
			&& card.Position.IsAdjacentTo(other.Position);
		public static bool IsAdjacentTo(this IGameCard card, Space space)
			=> card.Location == Location.Board
			&& card.Position.IsAdjacentTo(space);

		/// <summary>
		/// Whether <paramref name="space"/> is in this card's AOE if this card is at <paramref name="mySpace"/>
		/// </summary>
		public static bool SpaceInAOE(this IGameCard card, Space space, Space mySpace)
			=> space != null
			&& mySpace != null
			&& card.SpellSubtypes != null
			&& card.SpellSubtypes.Any(s => s switch
			{
				CardBase.RadialSubtype => mySpace.DistanceTo(space) <= card.Radius,
				_ => false
			});
		public static bool SpaceInAOE(Space space) => SpaceInAOE(space, Position);
		/// <summary>
		/// Whether <paramref name="c"/> is in this card's AOE if this card is at <paramref name="mySpace"/>
		/// </summary>
		public static bool CardInAOE(GameCardBase c, Space mySpace) => SpaceInAOE(c.Position, mySpace);
		/// <summary>
		/// Whether <paramref name="c"/> is in the aoe of <see cref="this"/> card.
		/// </summary>
		public static bool CardInAOE(GameCardBase c) => CardInAOE(c, Position);
		/// <summary>
		/// Whether <paramref name="c"/> and this card have any spaces shared between their AOEs,
		/// if this card is at <paramref name="mySpace"/>
		/// </summary>
		public static bool Overlaps(GameCardBase c, Space mySpace) => Space.Spaces.Any(s => SpaceInAOE(s, mySpace) && c.SpaceInAOE(s));
		/// <summary>
		/// Whether <paramref name="c"/> and this card have any spaces shared between their AOEs
		/// </summary>
		public static bool Overlaps(GameCardBase c) => Overlaps(c, Position);

		public static bool SameColumn(Space space) => Location == Location.Board && Position.SameColumn(space);
		public static bool SameColumn(GameCardBase c) => c.Location == Location.Board && SameColumn(c.Position);

		/// <summary>
		/// Returns whether the <paramref name="space"/> passed in is in front of this card
		/// </summary>
		/// <param name="space">The space to check if it's in front of this card</param>
		/// <returns><see langword="true"/> if <paramref name="space"/> is in front of this, <see langword="false"/> otherwise.</returns>
		public static bool SpaceInFront(Space space) => ControllingPlayer.SubjectiveCoords(space).NorthOf(SubjectivePosition);

		/// <summary>
		/// Returns whether the card passed in is in front of this card
		/// </summary>
		/// <param name="card">The card to check if it's in front of this one</param>
		/// <returns><see langword="true"/> if <paramref name="card"/> is in front of this, <see langword="false"/> otherwise.</returns>
		public static bool CardInFront(GameCardBase card) => SpaceInFront(card.Position);

		/// <summary>
		/// Returns whether the <paramref name="space"/> passed in is behind this card
		/// </summary>
		/// <param name="space">The space to check if it's behind this card</param>
		/// <returns><see langword="true"/> if <paramref name="space"/> is behind this, <see langword="false"/> otherwise.</returns>
		public static bool SpaceBehind(Space space) => SubjectivePosition.NorthOf(ControllingPlayer.SubjectiveCoords(space));

		/// <summary>
		/// Returns whether the card passed in is behind this card
		/// </summary>
		/// <param name="card">The card to check if it's behind this one</param>
		/// <returns><see langword="true"/> if <paramref name="card"/> is behind this, <see langword="false"/> otherwise.</returns>
		public static bool CardBehind(GameCardBase card) => SpaceBehind(card.Position);

		public static bool SpaceDirectlyInFront(Space space)
			=> Location == Location.Board && ControllingPlayer.SubjectiveCoords(space) == SubjectivePosition.DueNorth;

		public static bool CardDirectlyInFront(GameCardBase card)
			=> card.Location == Location.Board && SpaceDirectlyInFront(card.Position);

		public static bool SameDiagonal(Space space) => Location == Location.Board && Position.SameDiagonal(space);
		public static bool SameDiagonal(GameCardBase card) => card?.Location == Location.Board && SameDiagonal(card.Position);

		public static bool InCorner() => Location == Location.Board && Position.IsCorner;

		/// <summary>
		/// Refers to this situation: <br></br>
		/// | <paramref name="space"/> | <br></br>
		/// | this card | <br></br>
		/// | <paramref name="card"/> param | <br></br>
		/// </summary>
		/// <param name="space">The space in the same axis as this card and <paramref name="card"/> param</param>
		/// <param name="card">The card in the same axis as this card and the <paramref name="space"/> param.</param>
		/// <returns></returns>
		public static bool SpaceDirectlyAwayFrom((int x, int y) space, GameCardBase card)
		{
			if (card.Location != Location.Board || Location != Location.Board) return false;
			int xDiffCard = card.Position.x - Position.x;
			int yDiffCard = card.Position.y - Position.y;
			int xDiffSpace = space.x - Position.x;
			int yDiffSpace = space.y - Position.y;

			return (xDiffCard == 0 && xDiffSpace == 0)
				|| (yDiffCard == 0 && yDiffSpace == 0)
				|| (xDiffCard == yDiffCard && xDiffSpace == yDiffSpace);
		}

		public static int ShortestPath(Space space, Predicate<GameCard> throughPredicate)
			=> Card.Game.Board.ShortestPath(Card.Position, space, throughPredicate);
	}

	/// <summary>
    /// Base class for card information relevant to a game.
    /// Could be the actual card itself, or a stashed copy of that card's information.
    /// Doesn't implement IGameCard because children should be able to decide whether these things have set accessors,
    /// and things that just want an IGameCard shouldn't assume that these have set accessors
    /// </summary>
	public abstract class GameCardBase : CardBase
	{
		#region immutable aspects
		public abstract GameCard Card { get; }

		public abstract int IndexInList { get; }
		public abstract Player OwningPlayer { get; }
		public abstract Player ControllingPlayer { get; }
		public abstract bool Summoned { get; }
		public abstract bool IsAvatar { get; }

		public abstract IReadOnlyCollection<GameCard> AdjacentCards { get; }

		public abstract IPlayRestriction PlayRestriction { get; }
		public abstract IMovementRestriction MovementRestriction { get; }
		/// <summary>
        /// When attacking, this restriction must be true of the defender.
        /// </summary>
		public abstract IRestriction<GameCardBase> AttackingDefenderRestriction { get; }
		#endregion

		#region mutable aspects
		public new int N
		{
			get => base.N;
			private set => base.N = value;
		}
		public new int E
		{
			get => base.E;
			private set => base.E = value;
		}
		public new int S
		{
			get => base.S;
			private set => base.S = value;
		}
		public new int W
		{
			get => base.W;
			private set => base.W = value;
		}
		public new int C
		{
			get => base.C;
			private set => base.C = value;
		}
		public new int A
		{
			get => base.A;
			private set => base.A = value;
		}

		public abstract Location Location { get; protected set; }
		public abstract GameCard AugmentedCard { get; protected set; }
		public abstract IReadOnlyCollection<GameCard> Augments { get; protected set; }
		/// <summary>
		/// Represents whether this card is currently known to the enemy of this player.
		/// TODO: make this also be accurate on client, remembering what thigns have been revealed
		/// </summary>
		public abstract bool KnownToEnemy { get; set; }

		public abstract bool Activated { get; protected set; }
		public abstract bool Negated { get; protected set; }
		public abstract int SpacesMoved { get; set; }
		public int SpacesCanMove => N - SpacesMoved;

		public abstract Space Position { get; set; }
		#endregion

		public bool Hurt => CardType == 'C' && Location == Location.Board && E < BaseE;

		#region distance/adjacency
		public Space SubjectivePosition => ControllingPlayer.SubjectiveCoords(Position);
		#endregion distance/adjacency

		public bool HasSubtype(string subtype) => SubtypeText.ToLower().Contains(subtype.ToLower());


		protected GameCardBase(CardStats stats,
							string subtext, string[] spellTypes,
							bool unique,
							int radius, int duration,
							char cardType, string cardName, string fileName,
							string effText,
							string subtypeText)
			: base(stats, subtext, spellTypes, unique, radius, duration, cardType, cardName, fileName, effText, subtypeText)
		{ }

		/* This must happen through setters, not properties, so that notifications and stack sending
		 * can be managed as intended. */
		public virtual void SetN(int n, IStackable stackSrc, bool onlyStatBeingSet = true) => N = n;
		public virtual void SetE(int e, IStackable stackSrc, bool onlyStatBeingSet = true) => E = e;
		public virtual void SetS(int s, IStackable stackSrc, bool onlyStatBeingSet = true) => S = s;
		public virtual void SetW(int w, IStackable stackSrc, bool onlyStatBeingSet = true) => W = w;
		public virtual void SetC(int c, IStackable stackSrc, bool onlyStatBeingSet = true) => C = c;
		public virtual void SetA(int a, IStackable stackSrc, bool onlyStatBeingSet = true) => A = a;

		protected override void SetStats(CardStats cardStats) => SetStats(cardStats, stackSrc: null);

		/// <summary>
		/// Shorthand for modifying a card's stats all at once.
		/// On the server, this only notifies the clients of stat changes once.
		/// </summary>
		public virtual void SetStats(CardStats stats, IStackable stackSrc = null)
		{
			SetN(stats.n, stackSrc, onlyStatBeingSet: false);
			SetS(stats.s, stackSrc, onlyStatBeingSet: false);
			SetW(stats.w, stackSrc, onlyStatBeingSet: false);
			SetC(stats.c, stackSrc, onlyStatBeingSet: false);
			SetA(stats.a, stackSrc, onlyStatBeingSet: false);
			//E goes last in case the character should die.
			SetE(stats.e, stackSrc, onlyStatBeingSet: false);
		}

		/// <summary>
		/// Shorthand for modifying a card's NESW all at once.
		/// On the server, this only notifies the clients of stat changes once.
		/// </summary>
		public virtual void SetCharStats(int n, int e, int s, int w, IStackable stackSrc = null)
		{
			SetN(n, stackSrc, onlyStatBeingSet: false);
			SetS(s, stackSrc, onlyStatBeingSet: false);
			SetW(w, stackSrc, onlyStatBeingSet: false);
			//E goes last in case the character should die.
			SetE(e, stackSrc, onlyStatBeingSet: false);
		}

		/// <summary>
		/// Shorthand for modifying a card's NESW all at once.
		/// On the server, this only notifies the clients of stat changes once.
		/// </summary>
		public void AddToCharStats(int n, int e, int s, int w, IStackable stackSrc = null)
			=> SetCharStats(N + n, E + e, S + s, W + w, stackSrc: stackSrc);

		/// <summary>
		/// Shorthand for modifying a card's stats all at once.
		/// On the server, this only notifies the clients of stat changes once.
		/// </summary>
		public void AddToStats(CardStats buff, IStackable stackSrc = null)
			=> SetStats(Stats + buff, stackSrc);

		public void SwapCharStats(GameCard other, bool swapN = true, bool swapE = true, bool swapS = true, bool swapW = true)
		{
			int[] aNewStats = new int[4];
			int[] bNewStats = new int[4];

			(aNewStats[0], bNewStats[0]) = swapN ? (other.N, N) : (N, other.N);
			(aNewStats[1], bNewStats[1]) = swapE ? (other.E, E) : (E, other.E);
			(aNewStats[2], bNewStats[2]) = swapS ? (other.S, S) : (S, other.S);
			(aNewStats[3], bNewStats[3]) = swapW ? (other.W, W) : (W, other.W);

			SetCharStats(aNewStats[0], aNewStats[1], aNewStats[2], aNewStats[3]);
			other.SetCharStats(bNewStats[0], bNewStats[1], bNewStats[2], bNewStats[3]);
		}
	}
}