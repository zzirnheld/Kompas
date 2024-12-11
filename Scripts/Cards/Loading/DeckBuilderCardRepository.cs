using Kompas.Cards.Models;
using Kompas.Effects.Models;

namespace Kompas.Cards.Loading
{
	public class DeckBuilderCardRepository : CardRepository
	{
		public DeckBuilderCardRepository()
			: base(IFileLoader.Godot, false)
		{
			Initialize();
		}

		public DeckBuilderCard CreateDeckBuilderCard(string cardName)
		{
			var json = GetJsonFromName(cardName)
				?? throw new System.NullReferenceException($"{cardName} doesn't correspond to a json");
			var serializableCard = SerializableCardFromJson(json)
				?? throw new System.NullReferenceException($"{json} couldn't be loaded");
			return CreateDeckBuilderCard(serializableCard);
		}

		public DeckBuilderCard CreateDeckBuilderCard(SerializableCard serializableCard)
		{
			_ = serializableCard.cardName ?? throw new System.NullReferenceException($"{serializableCard} had no name");
			var (_, elseText) = Enhance(serializableCard.effText ?? string.Empty, System.Array.Empty<IEffect>());
			return new(serializableCard, cardFileNames[serializableCard.cardName], elseText, this);
		}
	}
}