using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public class FromCardValue : ContextualParentIdentityBase<int>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<GameCardBase> card;
		[JsonProperty(Required = Required.Always)]
		public CardValue cardValue;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card.Initialize(initializationContext);
			cardValue.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> cardValue.GetValueOf(card.From(context, secondaryContext));
	}
	
	public class CardValue : ContextInitializeableBase
	{
		#region values
		public const string Nimbleness = "N";
		public const string Endurance = "E";
		public const string SummoningCost = "S";
		public const string Wounding = "W";
		public const string CastingCost = "C";
		public const string AugmentCost = "A";

		public const string Cost = "Cost";
		public const string NumberOfAugments = "Number of Augments";
		public const string DistanceToSource = "Distance to Source";
		public const string Index = "Index";
		public const string SpacesCanMove = "Spaces Can Move";
		#endregion values

		[JsonProperty(Required = Required.Always)]
		public string value;
		[JsonProperty]
		public int multiplier = 1;
		[JsonProperty]
		public int divisor = 1;
		[JsonProperty]
		public int modifier = 0;

		public GameCard Source => InitializationContext.source;

		//FUTURE: Make this more definitive
		public string DisplayName => value;

		public int GetValueOf(GameCardBase card)
		{
			ComplainIfNotInitialized();
			if (card == null) throw new NullCardException("Cannot get value of null card");

			int intermediateValue = value switch
			{
				Nimbleness => card.N,
				Endurance => card.E,
				SummoningCost => card.S,
				Wounding => card.W,
				CastingCost => card.C,
				AugmentCost => card.A,

				Cost => card.Cost,
				NumberOfAugments => card.Augments.Count,
				DistanceToSource => card.DistanceTo(Source),
				Index => card.IndexInList,
				SpacesCanMove => card.SpacesCanMove,
				
				_ => throw new System.ArgumentException($"Invalid value string {value}", nameof(value)),
			};
			return intermediateValue * multiplier / divisor + modifier;
		}

		public void SetValueOf(GameCard card, int num, IStackable stackSrc = null)
		{
			if (card == null) throw new System.ArgumentException("Cannot set value of null card", nameof(card));

			int intermediateValue = num * multiplier / divisor + modifier;
			switch (value)
			{
				case Nimbleness:
					card.SetN(intermediateValue, stackSrc: stackSrc);
					break;
				case Endurance:
					card.SetE(intermediateValue, stackSrc: stackSrc);
					break;
				case SummoningCost:
					card.SetS(intermediateValue, stackSrc: stackSrc);
					break;
				case Wounding:
					card.SetW(intermediateValue, stackSrc: stackSrc);
					break;
				case CastingCost:
					card.SetC(intermediateValue, stackSrc: stackSrc);
					break;
				case AugmentCost:
					card.SetA(intermediateValue, stackSrc: stackSrc);
					break;
				default:
					throw new System.ArgumentException($"Can't set {value} of a card!");
			}
		}
	}
}