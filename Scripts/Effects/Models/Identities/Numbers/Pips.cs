using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public class Pips : ContextualParentIdentityBase<int>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IPlayer> player;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> player.From(context, secondaryContext).Pips;
	}
}