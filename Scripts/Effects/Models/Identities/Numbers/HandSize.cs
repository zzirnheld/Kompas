using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public class HandSize : ContextualParentIdentityBase<int>
	{
		[JsonProperty]
		public IIdentity<Player> player = new Players.TargetIndex();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> player.From(context, secondaryContext).hand.HandSize;
	}
	
	public class HandSizeLimit : ContextualParentIdentityBase<int>
	{
		[JsonProperty]
		public IIdentity<Player> player = new Players.TargetIndex();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> player.From(context, secondaryContext).HandSizeLimit;
	}
}