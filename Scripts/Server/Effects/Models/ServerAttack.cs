using System.Threading.Tasks;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Gamestate;
using Kompas.Server.Networking;

namespace Kompas.Server.Effects.Models
{
	public class ServerAttack : Attack, IServerStackable
	{
		public ServerGame ServerGame { get; init; }

		private ServerStackController EffCtrl => ServerGame.StackController;

		public ServerAttack(ServerGame serverGame, IPlayer instigator, GameCard attacker, GameCard defender)
			: base(instigator, attacker, defender)
		{
			ServerGame = serverGame
				?? throw new System.ArgumentNullException(nameof(serverGame), "Server game cannot be null for attack");
		}

		/// <summary>
		/// Trigger the triggers related to attack declaration.
		/// Should be called before the attack is resolved.
		/// </summary>
		public void Declare(IStackable? stackSrc)
		{
			ServerNotifier.NotifyAttackStarted(instigator, attacker, defender);

			var attackerContext = new TriggeringEventContext(game: ServerGame, cardBefore: attacker, secondaryCardBefore: defender, 
				stackableCause: stackSrc, stackableEvent: this, eventCauseOverride: attacker, player: instigator);
			var defenderContext = new TriggeringEventContext(game: ServerGame, cardBefore: defender, secondaryCardBefore: attacker, 
				stackableCause: stackSrc, stackableEvent: this, eventCauseOverride: attacker, player: instigator);
			attackerContext.CacheCardInfoAfter();
			defenderContext.CacheCardInfoAfter();
			EffCtrl.TriggerForCondition(Trigger.Attacks, attackerContext);
			EffCtrl.TriggerForCondition(Trigger.Defends, defenderContext);
			EffCtrl.TriggerForCondition(Trigger.Battles, attackerContext, defenderContext);
		}

		//this is factored out so i can maybe eventually add some indication of whether an attack is still gonna be valid
		private bool StillValidAttack
		{
			get => attacker.Location == Location.Board
				&& defender.Location == Location.Board;
		}

		public Task StartResolution(IServerResolutionContext context)
		{
			var attackerContext = new TriggeringEventContext(game: ServerGame, cardBefore: attacker, secondaryCardBefore: defender, 
				stackableCause: this, stackableEvent: this, eventCauseOverride: attacker, player: instigator);
			var defenderContext = new TriggeringEventContext(game: ServerGame, cardBefore: defender, secondaryCardBefore: attacker, 
				stackableCause: this, stackableEvent: this, eventCauseOverride: attacker, player: instigator);
			if (StillValidAttack)
			{
				//deal the damage
				DealDamage();
				attackerContext.CacheCardInfoAfter();
				defenderContext.CacheCardInfoAfter();
			}
			EffCtrl.TriggerForCondition(Trigger.BattleEnds, attackerContext, defenderContext);
			//then finish the resolution by just returning that completed the task. (don't need to call anything)
			return Task.CompletedTask;
		}

		private void DealDamage()
		{
			//get damage from both, before either takes any damage, in case effects matter on hp
			int attackerDmg = attacker.CombatDamage;
			int defenderDmg = defender.CombatDamage;
			var attackerDealContext = new TriggeringEventContext(game: ServerGame, cardBefore: attacker, secondaryCardBefore: defender,
				stackableCause: this, stackableEvent: this, player: instigator, x: attackerDmg);
			var defenderDealContext = new TriggeringEventContext(game: ServerGame, cardBefore: defender, secondaryCardBefore: attacker,
				stackableCause: this, stackableEvent: this, player: instigator, x: defenderDmg);
			var attackerTakeContext = new TriggeringEventContext(game: ServerGame, cardBefore: attacker, secondaryCardBefore: defender,
				stackableCause: this, stackableEvent: this, player: instigator, x: defenderDmg);
			var defenderTakeContext = new TriggeringEventContext(game: ServerGame, cardBefore: defender, secondaryCardBefore: attacker,
				stackableCause: this, stackableEvent: this, player: instigator, x: attackerDmg);
			//deal the damage
			defender.TakeDamage(attackerDmg, stackSrc: this);
			attacker.TakeDamage(defenderDmg, stackSrc: this);
			attackerDealContext.CacheCardInfoAfter();
			defenderDealContext.CacheCardInfoAfter();
			attackerTakeContext.CacheCardInfoAfter();
			defenderTakeContext.CacheCardInfoAfter();
			//trigger effects based on combat damage
			EffCtrl.TriggerForCondition(Trigger.TakeCombatDamage, attackerTakeContext, defenderTakeContext);
			EffCtrl.TriggerForCondition(Trigger.DealCombatDamage, attackerDealContext, defenderDealContext);
		}
	}
}