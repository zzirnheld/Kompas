using System.Linq;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class AllOf : AllOfBase<IGameCardInfo>
	{
		public override string ToString() => $"Card Restriction of {InitializationContext.source?.CardName}." +
			$"\nRestriction Elements: {string.Join(", ", elements.Select(r => r))}";
	}

	public class Not : CardRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IRestriction<IGameCardInfo> negated;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			negated.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IGameCardInfo item, IResolutionContext context)
			=> !negated.IsValid(item, context);
	}

	public class CardExists : CardRestrictionBase
	{
		protected override bool IsValidLogic(IGameCardInfo card, IResolutionContext context)
			=> card != null;
	}
}