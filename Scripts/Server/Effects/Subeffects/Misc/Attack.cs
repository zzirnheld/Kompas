﻿using Kompas.Gamestate.Exceptions;
using Kompas.Server.Gamestate.Players;
using System;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Attack : ServerSubeffect
	{
		public int attackerIndex = -2;

		public override Task<ResolutionInfo> Resolve()
		{
			var attacker = Effect.GetTarget(attackerIndex);
			var defender = CardTarget;
			if (attacker == null)
				throw new NullCardException("Attacker was null");
			else if (defender == null)
				throw new NullCardException("Defender was null");

			var atk = ServerGame.Attack(attacker, defender,
				instigator: PlayerTarget as ServerPlayer ?? throw new InvalidOperationException(),
				stackSrc: Effect);
			Effect.StackableTargets.Add(atk);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}