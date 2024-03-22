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
		public IGameCard this[int index] { get; }
		public int HandSize { get; }
	}

	public interface IHand<TCard, TPlayer> : ILocationModel<TCard, TPlayer>, IHand
		where TCard : class, IGameCard<TCard, TPlayer>
		where TPlayer : IPlayer<TCard, TPlayer>
	{
		public new TCard this[int index] { get; }

		public void Add(TCard card, int? index = null, IStackable? stackableCause = null);
	}

	public abstract class Hand<TCard, TPlayer> : OwnedLocationModel<TCard, TPlayer>, IHand<TCard, TPlayer>
		where TCard : class, IGameCard<TCard, TPlayer>
		where TPlayer : IPlayer<TCard, TPlayer>
	{
		private readonly IList<TCard> hand = new List<TCard>();
		public override IEnumerable<TCard> Cards => hand;

		public override Location Location => Location.Hand;

		private readonly HandController handController;

		public int HandSize => hand.Count;

		protected Hand(TPlayer owner, HandController handController) : base(owner)
		{
			this.handController = handController;
			handController.HandModel = this; //TODO: is there another, better way to initialize HandModel? without leaking this
		}

		public TCard this[int index] => hand[index];
		IGameCard IHand.this[int index] => this[index];

		public override int IndexOf(TCard card) => hand.IndexOf(card);

		protected override void PerformAdd(TCard card, int? index, IStackable? stackableCause)
		{
			GD.Print($"Adding {card}");
			if (index.HasValue) hand.Insert(index.Value, card);
			else hand.Add(card);
			handController.Refresh();
		}

		public override void Remove(TCard card)
		{
			if (!hand.Contains(card)) throw new CardNotHereException(Location, card,
				$"Hand of \n{string.Join(", ", hand.Select(c => c.CardName))}\n doesn't contain {card}, can't remove it!");

			hand.Remove(card);
			handController.Refresh();
		}
	}
}