using System;
using Kompas.Cards.Models;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Cards
{

	public class Avatar : ContextualParentIdentityBase<IGameCardInfo>
	{
		#nullable disable
		[JsonProperty (Required = Required.Always)]
		public IIdentity<IPlayer> player;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override IGameCardInfo? AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var card = player.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			return card.Avatar;
		}
	}
}