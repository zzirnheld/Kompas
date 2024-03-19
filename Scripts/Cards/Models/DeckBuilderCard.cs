namespace Kompas.Cards.Models
{
	public class DeckBuilderCard : CardBase
	{
		public override int BaseN { get; }
		public override int BaseE { get; }
		public override int BaseS { get; }
		public override int BaseW { get; }
		public override int BaseC { get; }
		public override int BaseA { get; }

		public override string BBCodeEffText { get; }

		public DeckBuilderCard(SerializableCard card, string fileName, string bbCodeEffText)
			: base((card.n, card.e, card.s, card.w, card.c, card.a),
					card.subtext, card.spellTypes,
					card.unique,
					card.radius, card.duration,
					card.cardType, card.cardName, fileName,
					card.effText,
					card.subtypeText)
		{
			BaseN = card.n;
			BaseE = card.e;
			BaseS = card.s;
			BaseW = card.w;
			BaseC = card.c;
			BaseA = card.a;

			BBCodeEffText = bbCodeEffText;
		}
	}
}