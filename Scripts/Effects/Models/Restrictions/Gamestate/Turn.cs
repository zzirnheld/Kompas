using Kompas.Effects.Models.Identities;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public abstract class Turn : GamestateRestrictionBase
	{
		//If end up needing a version that can leverage trigger restriction elements, will need to split this back out to trigger/gamestate versions
		protected abstract IIdentity<IPlayer> TurnPlayer { get; }

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			TurnPlayer.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IResolutionContext context)
			=> InitializationContext.game.TurnPlayer == TurnPlayer.From(context);

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context)
			=> true;
	}

	public class FriendlyTurn : Turn
	{
		private readonly IIdentity<IPlayer> turnPlayer = new Identities.Players.FriendlyPlayer();
		protected override IIdentity<IPlayer> TurnPlayer => turnPlayer;
	}

	public class EnemyTurn : Turn
	{
		private readonly IIdentity<IPlayer> turnPlayer = new Identities.Players.EnemyPlayer();
		protected override IIdentity<IPlayer> TurnPlayer => turnPlayer;
	}
}