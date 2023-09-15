using System.Collections.Generic;
using Kompas.Effects.Models.Selectors;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Spaces
{
	public class SelectFromMany : ContextualParentIdentityBase<Space>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<Space>> spaces;
		[JsonProperty(Required = Required.Always)]
		public ISelector<Space> selector;// = new RandomSelector<Space>();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			spaces.Initialize(initializationContext);
		}

		protected override Space AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> selector.Select(spaces.From(context, secondaryContext));
	}
}