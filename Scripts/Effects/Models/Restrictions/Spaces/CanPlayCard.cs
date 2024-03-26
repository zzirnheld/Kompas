using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;
using Kompas.Cards.Models;
using Kompas.Gamestate;
using System;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	/// <summary>
	/// Whether a card can be moved to that space. Presumes from effect
	/// </summary>
	public class CanPlayCard : SpaceRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IGameCardInfo> toPlay;
		#nullable restore

		[JsonProperty]
		public bool ignoreAdjacency;

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			toPlay.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space? space, IResolutionContext context)
		{
			var restriction = (toPlay.From(context)?.PlayRestriction)
				?? throw new InvalidOperationException();
			return ignoreAdjacency
				? restriction.IsValidIgnoringAdjacency((space, InitializationContext.Owner), context)
				: restriction.IsValid((space, InitializationContext.Owner), context);
		}
	}
}