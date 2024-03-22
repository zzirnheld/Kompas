using System.Collections.Generic;
using Kompas.Cards.Controllers;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions.Cards;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;

namespace Kompas.Cards.Models
{
	public interface IGameCard : IGameCardInfo
	{
		public int ID { get; }
		public string BaseJson { get; }

		public GameCardLinksModel CardLinkHandler { get; }

		public ICardController CardController { get; }

		public ILocationModel LocationModel { get; }
		public new Space? Position { get; set; }
		public bool InHiddenLocation { get; }

		public IPlayer OwningPlayer { get; }

		public int SpacesMoved { get; set; }
		public int AttacksThisTurn { get; set; }
		public int TurnsOnBoard { get; set; }
		public new int Duration { get; set; }
		public IEnumerable<IGameCard> AdjacentCards { get; }

		public void Remove(IStackable? stackSrc = null);
		public void CountSpacesMovedTo(Space space);

		public void SetNegated(bool negated, IStackable? stackSrc = null);

		public void SetN(int n, IStackable? stackSrc = null, bool onlyStatBeingSet = true);
		public void SetE(int e, IStackable? stackSrc = null, bool onlyStatBeingSet = true);
		public void SetS(int s, IStackable? stackSrc = null, bool onlyStatBeingSet = true);
		public void SetW(int w, IStackable? stackSrc = null, bool onlyStatBeingSet = true);
		public void SetC(int c, IStackable? stackSrc = null, bool onlyStatBeingSet = true);
		public void SetA(int a, IStackable? stackSrc = null, bool onlyStatBeingSet = true);

		public void SetCharStats(int n, int e, int s, int w, IStackable? stackSrc = null);
		public void SetStats(CardStats stats, IStackable? stackSrc = null);
		public void TakeDamage(int damage, IStackable? stackSrc = null);
	}

	public interface IGameCard<CardType, PlayerType> : IGameCard
		where CardType : class, IGameCard<CardType, PlayerType>
		where PlayerType : IPlayer<CardType, PlayerType>
	{
		public new CardType Card { get; }

		public new IGame<CardType, PlayerType> Game { get; }

		public new ILocationModel<CardType, PlayerType> LocationModel { get; set; }
		public new PlayerType OwningPlayer { get; }
		public new PlayerType ControllingPlayer { get; }

		public new IEnumerable<CardType> Augments { get; }
		//Interesting note on why this isn't a protected set:
		//C# can't know at compile time that we'll be the same inheritor of IGameCard<C, P>
		//that is trying to set AugmentedCard as the type being set on.
		//Therefore, it treats those as two different "protected"s.
		public new CardType? AugmentedCard { get; set; }

		public void AddAugment(CardType card, IStackable? stackSrc = null);

		/// <summary>
        /// Removes the augment.
        /// This can't be protected because any given card doesn't know for sure that it's in the other cards' inheritance hierarchy.
        /// </summary>
		public void RemoveAugment(CardType card);
	}
}