using System;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Cards
{

	public class AugmentedCard : ContextualParentIdentityBase<IGameCardInfo>
	{
		#nullable disable
		[JsonProperty (Required = Required.Always)]
		public IIdentity<IGameCardInfo> ofThisCard;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			ofThisCard.Initialize(initializationContext);
		}

		protected override IGameCardInfo? AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var card = ofThisCard.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			return card.AugmentedCard;
		}
	}
}