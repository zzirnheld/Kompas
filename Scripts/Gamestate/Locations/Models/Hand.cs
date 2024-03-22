using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models
{
	public interface IHand : ILocationModel
	{
		public int HandSize { get; }
	}

	public interface IHand<CardType, PlayerType> : ILocationModel<CardType, PlayerType>, IHand
		where CardType : class, IGameCard<CardType, PlayerType>
		where PlayerType : IPlayer<CardType, PlayerType>
	{ }

	public abstract class Hand<CardType, PlayerType> : OwnedLocationModel<CardType, PlayerType>, IHand<CardType, PlayerType>
		where CardType : class, IGameCard<CardType, PlayerType>
		where PlayerType : IPlayer<CardType, PlayerType>
	{
		private readonly IList<CardType> hand = new List<CardType>();
		public override IEnumerable<CardType> Cards => hand;

		public override Location Location => Location.Hand;

		private readonly HandController handController;

		public int HandSize => hand.Count;

		protected Hand(PlayerType owner, HandController handController) : base(owner)
		{
			this.handController = handController;
			handController.HandModel = this; //TODO: is there another, better way to initialize HandModel? without leaking this
		}

		public CardType this[int index] => hand[index];

		public override int IndexOf(CardType card) => hand.IndexOf(card);

		protected override void PerformAdd(CardType card, int? index, IStackable? stackableCause)
		{
			GD.Print($"Adding {card}");
			if (index.HasValue) hand.Insert(index.Value, card);
			else hand.Add(card);
			handController.Refresh();
		}

		public override void Remove(CardType card)
		{
			if (!hand.Contains(card)) throw new CardNotHereException(Location, card,
				$"Hand of \n{string.Join(", ", hand.Select(c => c.CardName))}\n doesn't contain {card}, can't remove it!");

			hand.Remove(card);
			handController.Refresh();
		}
	}
}