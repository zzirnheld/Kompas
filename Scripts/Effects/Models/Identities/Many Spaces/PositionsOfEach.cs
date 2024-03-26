using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Shared.Enumerable;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManySpaces
{
	public class PositionsOfEach : ContextualParentIdentityBase<IReadOnlyCollection<Space>>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<IGameCardInfo>> cards;
		#nullable restore

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cards.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<Space> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var cards = this.cards.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			return cards
				.SelectMany(c => EnumerableHelper.YieldNonNull(c.Position))
				.ToArray();
		}
	}
}