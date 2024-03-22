using System;
using System.Linq;
using Godot;
using Kompas.Cards.Loading;

namespace Kompas.Cards.Models
{
	public interface ICard : IComparable
	{
		public string CardName { get; }
		public char Type { get; }

		public int N { get; }
		public int E { get; }
		public int S { get; }
		public int W { get; }
		public int C { get; }
		public int A { get; }

		public int Cost { get; }

		public string SubtypeText { get; }
		public string EffText { get; }
		public string BBCodeEffText { get; }

		public Texture2D? CardFaceImage { get; }
	}

	public abstract class CardBase : ICard
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

		public virtual int BaseN => N;
		public virtual int BaseE => E;
		public virtual int BaseS => S;
		public virtual int BaseW => W;
		public virtual int BaseC => C;
		public virtual int BaseA => A;
		public CardStats Stats => (N, E, S, W, C, A);

		public bool Unique { get; private set; }

		public string Subtext { get; private set; } = string.Empty;
		public string[] SpellSubtypes { get; private set; } = System.Array.Empty<string>();
		public int Radius { get; private set; }
		public int Duration { get; set; }
		public char Type { get; private set; }
		public string CardName { get; private set; } = string.Empty;
		public string SubtypeText { get; private set; } = string.Empty;

		protected event EventHandler<string>? EffTextChanged;
		private string _effText = string.Empty;
		public string EffText
		{
			get => _effText;
			set
			{
				_effText = value;
				EffTextChanged?.Invoke(this, value);
			}
		}
		public virtual string BBCodeEffText => EffText;

		public string QualifiedSubtypeText => AttributesString + ArgsString + SubtypeText;

		public int Cost
		{
			get
			{
				return Type switch
				{
					'C' => S,
					'S' => C,
					'A' => A,
					_ => throw new System.NotImplementedException($"Cost not implemented for card type {Type}"),
				};
			}
		}
		private string ArgsString
		{
			get
			{
				if (Type == 'S')
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
				return Type switch
				{
					'C' => $"N: {N} / E: {E} / S: {S} / W: {W}",
					'S' => $"C {C}",
					'A' => $"A {A}",
					_ => throw new System.NotImplementedException($"Stats string not implemented for card type {Type}"),
				};
			}
		}
		#endregion

		public Texture2D? CardFaceImage { get; private set; }

		public virtual string FileName { get; set; }

		protected CardBase(CardStats stats,
									   string? subtext, string[] spellTypes,
									   bool unique,
									   int radius, int duration,
									   char cardType, string? cardName, string? fileName,
									   string? effText,
									   string? subtypeText)
		{
			(n, e, s, w, c, a) = stats;

			FileName = fileName
				?? throw new System.NullReferenceException($"{cardName} had no associated file?");
			SetInfo(null, subtext, spellTypes, unique, radius, duration, cardType, cardName, effText, subtypeText);
		}

		protected void SetInfo(CardStats? stats,
									   string? subtext, string[] spellTypes,
									   bool unique,
									   int radius, int duration,
									   char cardType, string? cardName,
									   string? effText,
									   string? subtypeText)
		{
			if (stats.HasValue) SetStats(stats.Value);

			//set sprites if they aren't already set correctly 
			//(check this by card name. cards should never have a pic that doesn't match their name)
			if (cardName != CardName) CardFaceImage = CardRepository.LoadSprite(FileName);
			else GD.Print("Names match. Set Info not updating pics.");

			Subtext = subtext ?? string.Empty; //TODO un-deprecate and use as an override for constructed subtype text from the subtypes array
			SpellSubtypes = spellTypes;
			Unique = unique;
			Radius = radius;
			Duration = duration;
			Type = cardType;
			CardName = cardName ?? throw new ArgumentNullException(nameof(cardName), $"A card is missing a name.");
			EffText = effText ?? throw new ArgumentNullException(nameof(effText), $"Card {CardName} is missing effect text");
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

		public int CompareTo(object? obj)
		{
			if (obj == null) return 1;

			if (obj is not CardBase other) throw new ArgumentException("Other object is not a CardBase!");

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