using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManySpaces
{
	public class PositionsOfEach : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<GameCardBase>> cards;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cards.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<Space> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> cards.From(context, secondaryContext)
					.Select(c => c.Position)
					.Where(space => space != null)
					.ToArray();
	}
}