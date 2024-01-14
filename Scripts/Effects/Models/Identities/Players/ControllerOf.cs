using System;
using Kompas.Cards.Models;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Players
{
	public class ControllerOf : ContextualParentIdentityBase<IPlayer>
	{
		#nullable disable
		[JsonProperty]
		public IIdentity<IGameCardInfo> card;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card?.Initialize(initializationContext);

			if (AllNull(card)) throw new System.ArgumentException($"Must provide something to check controller of");
		}

		protected override IPlayer AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			if (this.card != null)
			{
				var card = this.card.From(context, secondaryContext)
					?? throw new InvalidOperationException();
				return card.ControllingPlayer;
			}
			throw new System.ArgumentException("huh?");
		}
	}
}