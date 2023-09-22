using Kompas.Cards.Models;

namespace Kompas.Cards.Loading
{
	public class DeckBuilderCardRepository : CardRepository
	{
		public static DeckBuilderCard CreateDeckBuilderCard(string cardName)
		{
			var json = GetJsonFromName(cardName);
			var serializableCard = SerializableCardFromJson(json);
			return CreateDeckBuilderCard(serializableCard);
		}

		public static DeckBuilderCard CreateDeckBuilderCard(SerializableCard serializableCard)
			=> new(serializableCard, cardFileNames[serializableCard.cardName]);
	}
}