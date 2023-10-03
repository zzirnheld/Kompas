using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class Concat : ContextualParentIdentityBase<IReadOnlyCollection<IGameCard>>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IGameCard>[] cards;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var i in cards) i.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<IGameCard> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> cards.Select(s => s.From(context, secondaryContext)).ToArray();
	}
}