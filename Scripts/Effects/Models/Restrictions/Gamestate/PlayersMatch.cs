using Kompas.Effects.Models.Identities;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class PlayersMatch : TriggerGamestateRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IPlayer> firstPlayer;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IPlayer> secondPlayer;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			firstPlayer.Initialize(initializationContext);
			secondPlayer.Initialize(initializationContext);
		}

		protected override bool IsValidContext(TriggeringEventContext context, IResolutionContext secondaryContext)
			=> firstPlayer.From(context, secondaryContext) == secondPlayer.From(context, secondaryContext);

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context, IResolutionContext dummyContext)
			=> true;
	}
}