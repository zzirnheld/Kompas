using System.Collections.Generic;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Cards.Models
{
	/// <summary>
	/// Base class for card information relevant to a game.
	/// Could be the actual card itself, or a stashed copy of that card's information.
	/// Doesn't implement IGameCard because children should be able to decide whether these things have set accessors,
	/// and things that just want an IGameCard shouldn't assume that these have set accessors
	/// </summary>
	public abstract class GameCardBase : CardBase
	{
		#region immutable aspects
		public abstract bool Summoned { get; }
		public abstract bool IsAvatar { get; }

		public abstract IEnumerable<IGameCard> AdjacentCards { get; }

		public abstract IPlayRestriction PlayRestriction { get; }
		public abstract IMovementRestriction MovementRestriction { get; }
		/// <summary>
		/// When attacking, this restriction must be true of the defender.
		/// </summary>
		public abstract IRestriction<IGameCardInfo> AttackingDefenderRestriction { get; }
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
		/// <summary>
		/// Represents whether this card is currently known to the enemy of this player.
		/// TODO: make this also be accurate on client, remembering what thigns have been revealed
		/// </summary>
		public abstract bool KnownToEnemy { get; set; }

		public abstract bool Activated { get; protected set; }
		public abstract bool Negated { get; protected set; }
		public abstract int SpacesMoved { get; set; }
		public int SpacesCanMove => N - SpacesMoved;

		public abstract Space? Position { get; set; }
		#endregion


		protected GameCardBase(CardStats stats,
							string? subtext, string[] spellTypes,
							bool unique,
							int radius, int duration,
							char TCard, string? cardName, string? fileName,
							string? effText,
							string? subtypeText)
			: base(stats, subtext, spellTypes, unique, radius, duration, TCard, cardName, fileName, effText, subtypeText)
		{}

		/* This must happen through setters, not properties, so that notifications and stack sending
		 * can be managed as intended. */
		public virtual void SetN(int n, IStackable? stackSrc, bool onlyStatBeingSet = true) => N = n;
		public virtual void SetE(int e, IStackable? stackSrc, bool onlyStatBeingSet = true) => E = e;
		public virtual void SetS(int s, IStackable? stackSrc, bool onlyStatBeingSet = true) => S = s;
		public virtual void SetW(int w, IStackable? stackSrc, bool onlyStatBeingSet = true) => W = w;
		public virtual void SetC(int c, IStackable? stackSrc, bool onlyStatBeingSet = true) => C = c;
		public virtual void SetA(int a, IStackable? stackSrc, bool onlyStatBeingSet = true) => A = a;

		protected override void SetStats(CardStats cardStats) => SetStats(cardStats, stackSrc: null);

		/// <summary>
		/// Shorthand for modifying a card's stats all at once.
		/// On the server, this only notifies the clients of stat changes once.
		/// </summary>
		public virtual void SetStats(CardStats stats, IStackable? stackSrc = null)
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
		public virtual void SetCharStats(int n, int e, int s, int w, IStackable? stackSrc = null)
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
		public void AddToCharStats(int n, int e, int s, int w, IStackable? stackSrc = null)
			=> SetCharStats(N + n, E + e, S + s, W + w, stackSrc: stackSrc);

		/// <summary>
		/// Shorthand for modifying a card's stats all at once.
		/// On the server, this only notifies the clients of stat changes once.
		/// </summary>
		public void AddToStats(CardStats buff, IStackable? stackSrc = null)
			=> SetStats(Stats + buff, stackSrc);
	}
}