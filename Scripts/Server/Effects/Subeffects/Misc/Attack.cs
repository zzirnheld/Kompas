using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Server.Gamestate.Players;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class AttackData : SubeffectData
{
	[JsonProperty]
	public int attackerIndex = -2;
}

public class Attack : ServerSubeffect
{
	private readonly int attackerIndex;

	public Attack(AttackData data) : base(data)
	{
		attackerIndex = data.attackerIndex;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var attacker = resolution.Context.GetCardTarget(attackerIndex)
			?? throw new NullCardException("Attacker was null");
		var defender = GetCardTarget(resolution.Context)
			?? throw new NullCardException("Defender was null");

		//TODO: is there a better way to cast it? / do we need it to be a ServerPlayer for the ServerAttack?
		var instigator = GetPlayerTarget(resolution.Context) as ServerPlayer
			?? throw new InvalidOperationException();

		var atk = ServerGame.Attack(attacker, defender,
			instigator: instigator,
			stackSrc: Effect);
		resolution.Context.StackableTargets.Add(atk);
		return Task.FromResult(ResolutionInfo.Next);
	}
}