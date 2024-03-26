using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Players;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Controller : CardRestrictionBase
	{
		#nullable disable
		[JsonProperty] //If not initialized here, should be initialized in child
		public IIdentity<IPlayer> playerIdentity;
		#nullable restore

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			playerIdentity.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IGameCardInfo? card, IResolutionContext context)
			=> card != null && playerIdentity.From(context) == card.ControllingPlayer;
	}

	public class Friendly : Controller
	{
		public override void Initialize(InitializationContext initializationContext)
		{
			playerIdentity = new FriendlyPlayer();
			base.Initialize(initializationContext);
		}
	}

	public class Enemy : Controller
	{
		public override void Initialize(InitializationContext initializationContext)
		{
			playerIdentity = new EnemyPlayer();
			base.Initialize(initializationContext);
		}
	}
}