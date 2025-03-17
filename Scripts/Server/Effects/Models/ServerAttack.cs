using System.Threading.Tasks;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;
using Kompas.Server.Effects.Controllers;
using Kompas.Server.Networking;

namespace Kompas.Server.Effects.Models;

public class ServerAttackResolution
    : ResolvingStackable<ServerAttack, IServerResolutionContext>,
        IServerStackableResolution<ServerAttack>
{
	private readonly IServerStackController stack;

	private ServerAttack Attack => Stackable;

	public ServerAttackResolution(ServerAttack stackable, IServerResolutionContext context,
		IServerStackController stack)
		: base(stackable, context)
	{
		this.stack = stack;
	}

	/// <summary>
	/// Trigger the triggers related to attack declaration.
	/// Should be called before the attack is resolved.
	/// </summary>
	public void Declare(IStackable? stackSrc)
	{
		ServerNotifier.NotifyAttackStarted(Attack.instigator, Attack.attacker, Attack.defender);

		var battlesContext = IEventContext.Build(Trigger.Battles)
			.CausedBy(stackSrc)
			.During(Attack)
			.CausedBy(Attack.attacker) //The attack itself is caused by the attacker
			.ForPlayer(Attack.instigator);
		var attackerBattles = battlesContext.Clone().AffectingBoth(Attack.attacker, Attack.defender);
		var defenderBattles = battlesContext.Clone().AffectingBoth(Attack.defender, Attack.attacker);
		var contexts = EventCapturer.Capture(() => { },
			attackerBattles.CloneForEvent(Trigger.Attacks),
			defenderBattles.CloneForEvent(Trigger.Defends),
			attackerBattles,
			defenderBattles);
		stack.TriggerFor(contexts);
	}

	//this is factored out so i can maybe eventually add some indication of whether an attack is still gonna be valid
	private bool StillValidAttack
	{
		get => Attack.attacker.Location == Location.Board
			&& Attack.defender.Location == Location.Board;
	}

	public Task StartResolution()
	{
		var battleEndsContext = IEventContext.Build(Trigger.BattleEnds)
			.CausedBy(Attack)
			.During(Attack)
			.CausedBy(Attack.attacker) //The attack itself is caused by the attacker
			.ForPlayer(Attack.instigator);
		var contexts = EventCapturer.Capture(
			() => { if (StillValidAttack) DealDamage(); },
			battleEndsContext.Clone().AffectingBoth(Attack.attacker, Attack.defender),
			battleEndsContext.Clone().AffectingBoth(Attack.defender, Attack.attacker)
		);
		stack.TriggerFor(contexts);
		//then finish the resolution by just returning that completed the task. (don't need to call anything)
		return Task.CompletedTask;
	}

	private void DealDamage()
	{
		//get damage from both, before either takes any damage, in case effects matter on hp
		int attackerDmg = Attack.attacker.CombatDamage;
		int defenderDmg = Attack.defender.CombatDamage;

		var baseContext = IEventContext.Build(Trigger.Anything)
			.CausedBy(Attack) //Damage, however, is caused by the card that did the damage, not by the card that initiated the attack.
			.During(Attack)
			.ForPlayer(Attack.instigator);

		var attackerBase = baseContext.Clone()
			.AffectingBoth(Attack.attacker, Attack.defender);
		var defenderBase = baseContext.Clone()
			.AffectingBoth(Attack.defender, Attack.attacker);

		var contexts = EventCapturer.Capture(
			() =>
			{
				Attack.defender.TakeDamage(attackerDmg, stackSrc: Attack);
				Attack.attacker.TakeDamage(defenderDmg, stackSrc: Attack);
			},
			attackerBase.CloneForEvent(Trigger.TakeCombatDamage).WithX(defenderDmg),
			defenderBase.CloneForEvent(Trigger.TakeCombatDamage).WithX(attackerDmg),
			attackerBase.CloneForEvent(Trigger.DealCombatDamage).WithX(attackerDmg),
			defenderBase.CloneForEvent(Trigger.DealCombatDamage).WithX(defenderDmg)
		);

		stack.TriggerFor(contexts);
	}
}

public class ServerAttack : Attack, IServerStackable
{
    public ServerAttack(IPlayer instigator, GameCard attacker, GameCard defender)
		: base(instigator, attacker, defender)
	{ }
}