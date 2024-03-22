using System.Collections.Generic;
using Kompas.Cards.Controllers;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;

namespace Kompas.Cards.Models
{
	public interface IGameCard : IGameCardInfo
	{
		public int ID { get; }

		public ICardController CardController { get; }
		public ILocationModel LocationModel { get; }

		public new Space? Position { get; set; }

		public void Remove(IStackable? stackSrc = null);
		public void CountSpacesMovedTo(Space space);
	}

	public interface IGameCard<CardType> : IGameCard
		where CardType : IGameCard<CardType>, IGameCard
	{
		public new CardType Card { get; }
		public new ILocationModel<CardType> LocationModel { get; set; }

		public new IReadOnlyCollection<CardType> Augments { get; }
		public new CardType? AugmentedCard { get; }

		public void AddAugment(CardType card, IStackable? stackSrc = null);
	}
}