using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models;

namespace Kompas.Cards.Controllers
{
	/// <summary>
    /// Controls the CardLinks relevant to a particular GameCard
    /// </summary>
	public class CardLinkController
	{
		public GameCard Card { get; }

		private readonly List<CardLink> links = new List<CardLink>();
		public IReadOnlyCollection<CardLink> Links => links;

		public CardLinkController(GameCard card)
		{
			Card = card;
		}

		public void AddLink(CardLink cardLink)
		{
			links.Add(cardLink);
		}

		public void CreateLink(IEnumerable<int> cardIDs, Effect effect, Color linkColor)
		{
			var cardLink = new CardLink(new HashSet<int>(cardIDs), effect, linkColor);
			foreach(var card in cardIDs.Select(Card.Game.GetCardWithID))
			{
				card?.CardLinkHandler.AddLink(cardLink);
			}
		}

		private void RemoveLink(CardLink cardLink) => links.Remove(cardLink);

		public void RemoveEquivalentLink(IEnumerable<int> cardIDs, Effect effect)
		{
			var equivLink = links.FirstOrDefault(link => link.Matches(cardIDs, effect));
			if (equivLink == default) return;

			foreach(var card in equivLink.CardIDs.Select(Card.Game.GetCardWithID))
			{
				card?.CardLinkHandler.RemoveLink(equivLink);
			}

			Card.CardController.RefreshLinks();
		}
	}
}