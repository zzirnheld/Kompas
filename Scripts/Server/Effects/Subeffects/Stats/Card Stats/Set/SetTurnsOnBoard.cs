﻿using Kompas.Effects.Models.Identities.Numbers;

namespace Kompas.Server.Effects.Subeffects
{
	public class SetTurnsOnBoard : SetCardStats
	{
		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			turnsOnBoard = new EffectX() { multiplier = xMultiplier, modifier = xModifier, divisor = xDivisor };
			base.Initialize(eff, subeffIndex);
		}
	}
}