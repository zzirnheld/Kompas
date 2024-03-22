using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations.Controllers;
using Kompas.Gamestate.Players;

namespace Kompas.Gamestate.Locations.Models
{
	public interface IDiscard : ILocationModel
	{ }

	public interface IDiscard<TCard, TPlayer>
		: ILocationModel<TCard, TPlayer>, IDiscard
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
	{
		public void Add(TCard card, int? index = null, IStackable? stackableCause = null);
	}

	public abstract class Discard<TCard, TPlayer>
		: OwnedLocationModel<TCard, TPlayer>, IDiscard<TCard, TPlayer>
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
	{
		private readonly DiscardController discardController;

		protected readonly IList<TCard> discard = new List<TCard>();

		public override Location Location => Location.Discard;
		public override IEnumerable<TCard> Cards => discard;

		protected Discard(TPlayer owner, DiscardController discardController) : base(owner)
		{
			this.discardController = discardController;
			discardController.DiscardModel = this;
		}

		protected override void PerformAdd(TCard card, int? index, IStackable? stackableCause)
		{
			if (index.HasValue) discard.Insert(index.Value, card);
			else discard.Add(card);
			discardController.Refresh();
		}

		public override void Remove(TCard card)
		{
			if (!discard.Contains(card)) throw new CardNotHereException(Location.Discard, card);

			discard.Remove(card);
			discardController.Refresh();
		}

		public override int IndexOf(TCard card) => discard.IndexOf(card);
	}
}