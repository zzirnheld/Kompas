using System.Linq;
using Godot;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class AllOf : AllOfBase<GameCardBase>
	{
		[JsonProperty]
		public string blurb;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			if (blurb != null) GD.PrintErr($"{GetType()} blurb is on the card restriction. move it to the card target of {initializationContext.source}");
		}

		public GameCard Source => InitializationContext.source;

		public override string ToString() => $"Card Restriction of {Source?.CardName}." +
			$"\nRestriction Elements: {string.Join(", ", elements.Select(r => r))}";
	}

	public class Not : CardRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IRestriction<GameCardBase> negated;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			negated.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
			=> !negated.IsValid(item, context);
	}

	public class CardExists : CardRestrictionBase
	{
		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> card != null;
	}
}