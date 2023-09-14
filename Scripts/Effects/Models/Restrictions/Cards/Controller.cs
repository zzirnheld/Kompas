using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Players;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Cards
{
	public class Controller : CardRestrictionElement
	{
		[JsonProperty]
		public IIdentity<Player> playerIdentity;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			playerIdentity.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
			=> playerIdentity.From(context) == card.ControllingPlayer;
	}

	public class Friendly : Controller
	{
		public override void Initialize(EffectInitializationContext initializationContext)
		{
			playerIdentity = new FriendlyPlayer();
			base.Initialize(initializationContext);
		}
	}

	public class Enemy : Controller
	{
		public override void Initialize(EffectInitializationContext initializationContext)
		{
			playerIdentity = new EnemyPlayer();
			base.Initialize(initializationContext);
		}
	}
}