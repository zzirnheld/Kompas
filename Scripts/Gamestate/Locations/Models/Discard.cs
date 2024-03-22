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

	public interface IDiscard<CardType> : ILocationModel<CardType>, IDiscard
		where CardType : GameCard
	{ }

	public abstract class Discard<CardType, PlayerType> : OwnedLocationModel<CardType, PlayerType>, IDiscard<CardType>
		where CardType : GameCard
		where PlayerType : IPlayer
	{
		private readonly DiscardController discardController;

		protected readonly IList<CardType> discard = new List<CardType>();

		public override Location Location => Location.Discard;
		public override IEnumerable<CardType> Cards => discard;

		protected Discard(PlayerType owner, DiscardController discardController) : base(owner)
		{
			this.discardController = discardController;
			discardController.DiscardModel = this;
		}

		protected override void PerformAdd(CardType card, int? index, IStackable? stackableCause)
		{
			if (index.HasValue) discard.Insert(index.Value, card);
			else discard.Add(card);
			discardController.Refresh();
		}

		public override void Remove(CardType card)
		{
			if (!discard.Contains(card)) throw new CardNotHereException(Location.Discard, card);

			discard.Remove(card);
			discardController.Refresh();
		}

		public override int IndexOf(CardType card) => discard.IndexOf(card);
	}
}