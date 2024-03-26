using System;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public class HandSize : ContextualParentIdentityBase<int>
	{
		[JsonProperty]
		public IIdentity<IPlayer> player = new Players.TargetIndex();

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var player = this.player.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			return player.Hand.HandSize;
		}
	}
	
	public class HandSizeLimit : ContextualParentIdentityBase<int>
	{
		[JsonProperty]
		public IIdentity<IPlayer> player = new Players.TargetIndex();

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var player = this.player.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			return player.HandSizeLimit;
		}
	}
}