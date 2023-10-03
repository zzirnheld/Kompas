using System.Collections.Generic;
using System.Linq;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;

namespace Kompas.Cards.Models
{

	/// <summary>
	/// Holds the info for a card at a given snapshot in time.
	/// Used for triggers.
	/// </summary>
	public class GameCardInfo : GameCardBase, IGameCard
	{
		#region immutable aspects
		public GameCard Card { get; }
		public Game Game { get; }

		public int IndexInList { get; }
		public IPlayer OwningPlayer { get; }
		public IPlayer ControllingPlayer { get; }
		public override bool Summoned { get; }
		public override bool IsAvatar { get; }

		public override IReadOnlyCollection<GameCard> AdjacentCards { get; }

		public override IPlayRestriction PlayRestriction { get; }
		public override IMovementRestriction MovementRestriction { get; }
		public override IRestriction<IGameCard> AttackingDefenderRestriction { get; }


		public override int BaseN { get; }
		public override int BaseE { get; }
		public override int BaseS { get; }
		public override int BaseW { get; }
		public override int BaseC { get; }
		public override int BaseA { get; }
		#endregion

		#region mutable aspects
		//Note for the unfamiliar: most of these have setters so that inheritors can have setters for the same property names without hiding
		public override Location Location { get; protected set; }
		public override GameCard AugmentedCard { get; protected set; }
		public override IReadOnlyCollection<GameCard> Augments { get; protected set; }
		public override bool KnownToEnemy { get; set; }

		public override bool Activated { get; protected set; }
		public override bool Negated { get; protected set; }
		public override int SpacesMoved { get; set; }
		public override Space Position { get; set; }
		#endregion

		/// <summary>
		/// Snapshots the information of a card.
		/// </summary>
		/// <param name="card">The card whose information to snapshot</param>
		/// <returns>A <see cref="GameCardInfo"/> whose information matches the current state of <paramref name="card"/>, 
		/// or null if <paramref name="card"/> is <see langword="null"/></returns>
		public static GameCardInfo CardInfoOf(GameCard card)
		{
			if (card == null) return null;

			return new GameCardInfo(card);
		}

		public GameCardInfo(GameCard card)
			: base(card.Stats,
						card.Subtext, card.SpellSubtypes,
						card.Unique,
						card.Radius, card.Duration,
						card.CardType, card.CardName, card.FileName,
						card.EffText,
						card.SubtypeText)
		{
			Card = card;
			Game = card.Game;
			
			Location = card.Location;
			IndexInList = card.IndexInList;
			ControllingPlayer = card.ControllingPlayer;
			OwningPlayer = card.OwningPlayer;
			Summoned = card.Summoned;
			IsAvatar = card.IsAvatar;
			AugmentedCard = card.AugmentedCard;
			Augments = card.Augments.ToArray();
			KnownToEnemy = card.KnownToEnemy;
			PlayRestriction = card.PlayRestriction;
			MovementRestriction = card.MovementRestriction;
			AttackingDefenderRestriction = card.AttackingDefenderRestriction;

			BaseN = card.BaseN;
			BaseE = card.BaseE;
			BaseS = card.BaseS;
			BaseW = card.BaseW;
			BaseC = card.BaseC;
			BaseA = card.BaseA;

			Activated = card.Activated;
			Negated = card.Negated;
			SpacesMoved = card.SpacesMoved;
			AdjacentCards = card.AdjacentCards.ToArray();
			Position = card.Position?.Copy;
		}

		public override string ToString()
		{
			return CardName;
		}
	}
}