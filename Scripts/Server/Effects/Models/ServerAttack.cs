using System.Threading.Tasks;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Gamestate;
using Kompas.Server.Networking;

namespace Kompas.Server.Effects.Models;

public class ServerAttack : Attack, IServerStackable
{
	public ServerGame ServerGame { get; init; }

	private IServerStackController EffCtrl => ServerGame.StackController;

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

		var battlesContext = IEventContext.Build(Trigger.Battles)
			.CausedBy(stackSrc)
			.During(this)
			.CausedBy(attacker) //The attack itself is caused by the attacker
			.ForPlayer(instigator);
		var attackerBattles = battlesContext.Clone().AffectingBoth(attacker, defender);
		var defenderBattles = battlesContext.Clone().AffectingBoth(defender, attacker);
		var contexts = EventCapturer.Capture(() => { },
			attackerBattles.CloneForEvent(Trigger.Attacks),
			defenderBattles.CloneForEvent(Trigger.Defends),
			attackerBattles,
			defenderBattles);
		EffCtrl.TriggerFor(contexts);
	}

	//this is factored out so i can maybe eventually add some indication of whether an attack is still gonna be valid
	private bool StillValidAttack
	{
		get => attacker.Location == Location.Board
			&& defender.Location == Location.Board;
	}

	public Task StartResolution(IServerResolutionContext context)
	{
		var battleEndsContext = IEventContext.Build(Trigger.BattleEnds)
			.CausedBy(this)
			.During(this)
			.CausedBy(attacker) //The attack itself is caused by the attacker
			.ForPlayer(instigator);
		var contexts = EventCapturer.Capture(
			() => { if (StillValidAttack) DealDamage(); },
			battleEndsContext.Clone().AffectingBoth(attacker, defender),
			battleEndsContext.Clone().AffectingBoth(defender, attacker)
		);
		EffCtrl.TriggerFor(contexts);
		//then finish the resolution by just returning that completed the task. (don't need to call anything)
		return Task.CompletedTask;
	}

	private void DealDamage()
	{
		//get damage from both, before either takes any damage, in case effects matter on hp
		int attackerDmg = attacker.CombatDamage;
		int defenderDmg = defender.CombatDamage;

		var baseContext = IEventContext.Build(Trigger.Anything)
			.CausedBy(this) //Damage, however, is caused by the card that did the damage, not by the card that initiated the attack.
			.During(this)
			.ForPlayer(instigator);

		var attackerBase = baseContext.Clone()
			.AffectingBoth(attacker, defender);
		var defenderBase = baseContext.Clone()
			.AffectingBoth(defender, attacker);

		var contexts = EventCapturer.Capture(
			() => {
				defender.TakeDamage(attackerDmg, stackSrc: this);
				attacker.TakeDamage(defenderDmg, stackSrc: this);
			},
			attackerBase.CloneForEvent(Trigger.TakeCombatDamage).WithX(defenderDmg),
			defenderBase.CloneForEvent(Trigger.TakeCombatDamage).WithX(attackerDmg),
			attackerBase.CloneForEvent(Trigger.DealCombatDamage).WithX(attackerDmg),
			defenderBase.CloneForEvent(Trigger.DealCombatDamage).WithX(defenderDmg)
		);

		EffCtrl.TriggerFor(contexts);
	}
}