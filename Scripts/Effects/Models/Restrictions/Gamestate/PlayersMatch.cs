using Kompas.Effects.Models.Identities;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class PlayersMatch : TriggerGamestateRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IPlayer> firstPlayer;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IPlayer> secondPlayer;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			firstPlayer.Initialize(initializationContext);
			secondPlayer.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
			=> firstPlayer.From(context, secondaryContext) == secondPlayer.From(context, secondaryContext);
	}
}