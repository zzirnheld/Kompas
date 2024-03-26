using System;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public class Pips : ContextualParentIdentityBase<int>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<IPlayer> player;
		#nullable restore

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var player = this.player.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			return player.Pips;
		}
	}
}