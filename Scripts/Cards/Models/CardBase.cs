using System;
using System.Linq;

namespace Kompas.Cards.Models
{
	public abstract class CardBase : IComparable
	{
		public const string SimpleSubtype = "Simple";
		public const string EnchantSubtype = "Enchant";
		public const string DelayedSubtype = "Delayed";
		public const string RadialSubtype = "Radial";
		public const string VanishingSubtype = "Vanishing";
		public const string Nimbleness = "N";
		public const string Endurance = "E";
		public const string SummoningCost = "S";
		public const string Wounding = "W";
		public const string CastingCost = "C";
		public const string AugmentCost = "A";
		public const string CostStat = "Cost";

		#region stats
		private int n;
		private int e;
		private int s;
		private int w;
		private int c;
		private int a;
		/// <summary>
		/// Nimbleness - spaces moveable per turn
		/// </summary>
		public virtual int N
		{
			get => n < 0 ? 0 : n;
			protected set => n = value;
		}
		/// <summary>
		/// Endurance - hit points
		/// </summary>
		public virtual int E
		{
			get => e < 0 ? 0 : e;
			protected set => e = value;
		}
		/// <summary>
		/// Summoning cost - character's pip cost
		/// </summary>
		public virtual int S
		{
			get => s < 0 ? 0 : s;
			protected set => s = value;
		}
		/// <summary>
		/// Wounding - damage
		/// </summary>
		public virtual int W
		{
			get => w < 0 ? 0 : w;
			protected set => w = value;
		}
		/// <summary>
		/// Casting cost - spell's pip cost
		/// </summary>
		public virtual int C
		{
			get => c < 0 ? 0 : c;
			protected set => c = value;
		}
		/// <summary>
		/// Augment cost - augment's pip cost
		/// </summary>
		public virtual int A
		{
			get => a < 0 ? 0 : a;
			protected set => a = value;
		}

		public abstract int BaseN { get; }
		public abstract int BaseE { get; }
		public abstract int BaseS { get; }
		public abstract int BaseW { get; }
		public abstract int BaseC { get; }
		public abstract int BaseA { get; }

		public CardStats Stats => (N, E, S, W, C, A);

		public bool Unique { get; private set; }

		public string Subtext { get; private set; }
		public string[] SpellSubtypes { get; private set; }
		public int Radius { get; private set; }
		public int Duration { get; set; }
		public char CardType { get; private set; }
		public string CardName { get; private set; }
		public string EffText { get; private set; }
		public string SubtypeText { get; private set; }

		public string QualifiedSubtypeText => AttributesString + ArgsString + SubtypeText;

		public int Cost
		{
			get
			{
				return CardType switch
				{
					'C' => S,
					'S' => C,
					'A' => A,
					_ => throw new System.NotImplementedException($"Cost not implemented for card type {CardType}"),
				};
			}
		}
		private string ArgsString
		{
			get
			{
				if (CardType == 'S')
				{
					return (SpellSubtypes.FirstOrDefault()) switch
					{
						RadialSubtype => $" Radius {Radius} ",
						DelayedSubtype => $" Delayed {Duration} ",
						VanishingSubtype => $" Vanishing {Duration} ",
						_ => "",
					};
				}

				return "";
			}
		}
		public string AttributesString => $"{(Unique ? "Unique " : "")}";
		public string StatsString
		{
			get
			{
				return CardType switch
				{
					'C' => $"N: {N} / E: {E} / S: {S} / W: {W}",
					'S' => $"C {C}",
					'A' => $"A {A}",
					_ => throw new System.NotImplementedException($"Stats string not implemented for card type {CardType}"),
				};
			}
		}
		#endregion

		//public Sprite SimpleSprite { get; private set; }

		public virtual string FileName { get; set; }

		protected CardBase(CardStats stats,
									   string subtext, string[] spellTypes,
									   bool unique,
									   int radius, int duration,
									   char cardType, string cardName, string fileName,
									   string effText,
									   string subtypeText)
		{
			(n, e, s, w, c, a) = stats;

			FileName = fileName;
			SetInfo(null, subtext, spellTypes, unique, radius, duration, cardType, cardName, effText, subtypeText);
		}

		protected void SetInfo(CardStats? stats,
									   string subtext, string[] spellTypes,
									   bool unique,
									   int radius, int duration,
									   char cardType, string cardName,
									   string effText,
									   string subtypeText)
		{
			if (stats.HasValue) SetStats(stats.Value);

			//set sprites if they aren't already set correctly 
			//(check this by card name. cards should never have a pic that doesn't match their name)
			if (cardName != CardName)
			{
				//GD.Print($"Names are different, changing card pics to match name {FileName}");
				//TODO
				//SimpleSprite = CardRepository.LoadSprite(FileName);
			}
			//else GD.Print("Names match. Set Info not updating pics.");

			Subtext = subtext; //TODO un-deprecate and use as an override for constructed subtype text from the subtypes array
			SpellSubtypes = spellTypes;
			Unique = unique;
			Radius = radius;
			Duration = duration;
			CardType = cardType;
			CardName = cardName ?? throw new ArgumentNullException("cardName", $"A card is missing a name.");
			EffText = effText ?? throw new ArgumentNullException("effText", $"Card {CardName} is missing effect text");
			SubtypeText = subtypeText ?? string.Empty;
		}

		protected void SetInfo(SerializableCard serializableCard)
			=> SetInfo((serializableCard.n, serializableCard.e, serializableCard.s, serializableCard.w, serializableCard.c, serializableCard.a),
				serializableCard.subtext, serializableCard.spellTypes,
				serializableCard.unique,
				serializableCard.radius, serializableCard.duration,
				serializableCard.cardType, serializableCard.cardName,
				serializableCard.effText, serializableCard.subtypeText);

		protected virtual void SetStats(CardStats cardStats)
		{
			(N, E, S, W, C, A) = cardStats;
		}

		public override string ToString()
		{
			if (CardName == null) return "Null Card";
			return $"{CardName}, {N}/{E}/{S}/{W}/{C}/{A}";
		}

		public int CompareTo(object obj)
		{
			if (obj == null) return 1;

			var other = obj as CardBase;
			if (other == null) throw new ArgumentException("Other object is not a CardBase!");

			int compare = CardName.CompareTo(other.CardName);
			if (compare != 0) return compare;

			compare = N.CompareTo(other.N);
			if (compare != 0) return compare;

			compare = E.CompareTo(other.E);
			if (compare != 0) return compare;

			compare = S.CompareTo(other.S);
			if (compare != 0) return compare;

			compare = W.CompareTo(other.W);
			if (compare != 0) return compare;

			compare = C.CompareTo(other.C);
			if (compare != 0) return compare;

			compare = A.CompareTo(other.A);
			if (compare != 0) return compare;

			return 0;
		}
	}
}