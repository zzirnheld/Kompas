using System.Collections.Generic;
using Kompas.Effects.Models.Selectors;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Spaces
{
	public class SelectFromMany : ContextualParentIdentityBase<Space>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<Space>> spaces;
		[JsonProperty(Required = Required.Always)]
		public ISelector<Space> selector;// = new RandomSelector<Space>();
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			spaces.Initialize(initializationContext);
		}

        protected override Space? AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
        {
            var spaces = this.spaces.From(context, secondaryContext)
				?? throw new System.InvalidOperationException();
            return selector.Select(spaces);
        }
    }
}