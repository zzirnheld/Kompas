using Kompas.Cards.Models;

namespace Kompas.Cards.Loading
{
	public class DeckBuilderCardRepository : CardRepository
	{
		public static DeckBuilderCard CreateDeckBuilderCard(string cardName)
		{
			var json = GetJsonFromName(cardName)
				?? throw new System.NullReferenceException($"{cardName} doesn't correspond to a json");
			var serializableCard = SerializableCardFromJson(json)
				?? throw new System.NullReferenceException($"{json} couldn't be loaded");
			return CreateDeckBuilderCard(serializableCard);
		}

		public static DeckBuilderCard CreateDeckBuilderCard(SerializableCard serializableCard)
		{
			_ = serializableCard.cardName ?? throw new System.NullReferenceException($"{serializableCard} had no name");
			return new(serializableCard, cardFileNames[serializableCard.cardName]);
		}
	}
}