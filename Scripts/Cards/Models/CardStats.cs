namespace Kompas.Cards.Models
{
	public readonly struct CardStats
	{
		public readonly int n;
		public readonly int e;
		public readonly int s;
		public readonly int w;
		public readonly int c;
		public readonly int a;

		public CardStats(int n, int e, int s, int w, int c, int a)
		{
			this.n = n;
			this.e = e;
			this.s = s;
			this.w = w;
			this.c = c;
			this.a = a;
		}

		public static CardStats Of(IGameCardInfo card) => new(card.N, card.E, card.S, card.W, card.C, card.A);

		public readonly void Deconstruct(out int n, out int e, out int s, out int w, out int c, out int a)
		{
			n = this.n;
			e = this.e;
			s = this.s;
			w = this.w;
			c = this.c;
			a = this.a;
		}

		public static CardStats operator +(CardStats a, CardStats b)
			=> (a.n + b.n, a.e + b.e, a.s + b.s, a.w + b.w, a.c + b.c, a.a + b.a);

		public static CardStats operator *(int m, CardStats stats)
			=> (m * stats.n, m * stats.e, m * stats.s, m * stats.w, m * stats.c, m * stats.a);

		public static CardStats operator /(CardStats dividend, CardStats divisor)
			=> (dividend.n / divisor.n, dividend.e / divisor.e, dividend.s / divisor.s, dividend.w / divisor.w,
				dividend.c / divisor.c, dividend.a / divisor.a);

		public static CardStats operator *(CardStats stats, int m) => m * stats;

		public static implicit operator CardStats((int n, int e, int s, int w, int c, int a) stats)
			=> new(stats.n, stats.e, stats.s, stats.w, stats.c, stats.a);

		/// <summary>
		/// Returns a CardStats where any non-null stat given replaces the value in this CardStats.
		/// Values that are null in the given stats default to this set of stats' values.
		/// </summary>
		public CardStats ReplaceWith((int? n, int? e, int? s, int? w, int? c, int? a) stats)
		{
			return (
				stats.n ?? n,
				stats.e ?? e,
				stats.s ?? s,
				stats.w ?? w,
				stats.c ?? c,
				stats.a ?? a
			);
		}
	}
}