using System;
using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Shared.Enumerable;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class CardsInPositions : ContextualParentIdentityBase<IReadOnlyCollection<IGameCardInfo>>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<Space>> positions;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			positions.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<IGameCardInfo> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var spaces = positions.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			return spaces.Select(InitializationContext.game.Board.GetCardAt)
				.SelectMany(c => EnumerableHelper.YieldNonNull(c))
				.ToArray();
		}
	}
}