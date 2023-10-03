using Kompas.Effects.Models.Identities;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Players
{
	public class Is : PlayerRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IPlayer> player;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IPlayer item, IResolutionContext context)
		 => item == player.From(context);
	}
}