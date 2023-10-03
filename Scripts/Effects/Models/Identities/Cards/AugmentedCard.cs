using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Cards
{

	public class AugmentedCard : ContextualParentIdentityBase<IGameCard>
	{
		[JsonProperty]
		public IIdentity<IGameCard> ofThisCard;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			ofThisCard.Initialize(initializationContext);
		}

		protected override IGameCard AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> ofThisCard.From(context, secondaryContext).AugmentedCard;
	}
}