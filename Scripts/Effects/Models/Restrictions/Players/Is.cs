using Kompas.Effects.Models.Identities;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Players
{
	public class Is : PlayerRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Player> player;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Player item, IResolutionContext context)
		 => item == player.From(context);
	}
}