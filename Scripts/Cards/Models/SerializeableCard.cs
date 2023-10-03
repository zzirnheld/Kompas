using System.Collections.Generic;
using Kompas.Effects.Models;
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

		public string[] keywords = System.Array.Empty<string>();
		public int[] keywordArgs = System.Array.Empty<int>();

		public IMovementRestriction movementRestriction = null;
		public IRestriction<IGameCard> attackingDefenderRestriction = null;
		public IPlayRestriction PlayRestriction = null;

		public int n;
		public int e;
		public int s;
		public int w;
		public int c;
		public string[] spellTypes = System.Array.Empty<string>();
		public int radius;
		public int duration;
		public int a;
		public bool fast;
		public bool unique;
		public string subtext;

		public CardStats Stats => (n, e, s, w, c, a);

		public override string ToString() => $"{cardName}";
	}

	public abstract class SerializableGameCard : SerializableCard
	{
		public abstract IEnumerable<Effect> Effects { get; }
	}
}