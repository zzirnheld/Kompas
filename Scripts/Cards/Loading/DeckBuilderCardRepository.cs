using Kompas.Cards.Models;

namespace Kompas.Cards.Loading
{
	public class DeckBuilderCardRepository : CardRepository
	{
		/*
		public GameObject deckBuilderCardPrefab;

		private static SerializableCard SerializableCardFromJson(string json)
		{
			try
			{
				//Debug.Log($"Deserializing {json}");
				return JsonConvert.DeserializeObject<SerializableCard>(json, cardLoadingSettings);
			}
			catch (System.ArgumentException e)
			{
				Debug.LogError($"{json} had argument exception {e.Message}");
			}
			return null;
		}
		
		// new version
		public KompasDeckbuilder.UI.DeckBuilderCardController InstantiateDeckBuilderCard(string json, DeckBuilderController deckBuilderController)
		{
			SerializableCard serializableCard = SerializableCardFromJson(json);
			if (serializableCard == null) return null;

			var card = Instantiate(deckBuilderCardPrefab).GetComponent<KompasDeckbuilder.UI.DeckBuilderCardController>();
			card.SetInfo(serializableCard, deckBuilderController, FileNameFor(serializableCard.cardName));
			return card;
		}
		*/

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