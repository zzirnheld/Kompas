using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.ManyCards
{
	public class CardsInPositions : ContextualParentIdentityBase<IReadOnlyCollection<IGameCard>>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IReadOnlyCollection<Space>> positions;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			positions.Initialize(initializationContext);
		}

		protected override IReadOnlyCollection<IGameCard> AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var spaces = positions.From(context, secondaryContext);
			return spaces.Select(InitializationContext.game.Board.GetCardAt).Where(s => s != null).ToArray();
		}
	}
}