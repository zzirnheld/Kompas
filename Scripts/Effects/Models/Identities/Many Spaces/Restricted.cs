using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManySpaces
{
	public class Restricted : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IRestriction<Space> restriction;
		#nullable restore
		
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<Space>> spaces = new ManySpaces.All();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			restriction.Initialize(initializationContext);
			spaces.Initialize(initializationContext);
		}

        protected override IReadOnlyCollection<Space> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
        {
			var spaces = this.spaces.From(context, secondaryContext)
				?? throw new InvalidOperationException();
            return spaces.Where(s => restriction.IsValid(s, ContextToConsider(context, secondaryContext))).ToList();
        }
    }
}