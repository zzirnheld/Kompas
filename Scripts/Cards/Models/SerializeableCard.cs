using Kompas.Effects.Models.Restrictions;

namespace Kompas.Cards.Models
{
	public class SerializableCard
	{
		//card type
		public char cardType;

		//perma-values
		public string cardName;
		public string effText;
		public string subtypeText;

		public string[] keywords = new string[0];
		public int[] keywordArgs = new int[0];

		public IMovementRestriction movementRestriction = null;
		public IRestriction<GameCardBase> attackingDefenderRestriction = null;
		public IPlayRestriction PlayRestriction = null;

		public int n;
		public int e;
		public int s;
		public int w;
		public int c;
		public string[] spellTypes = { };
		public int radius;
		public int duration;
		public int a;
		public bool fast;
		public bool unique;
		public string subtext;

		public CardStats Stats => (n, e, s, w, c, a);
	}
}