using Kompas.Effects.Models.Identities.Numbers;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class ChangeCardStats : UpdateCardStats
	{
		public int nModifier = 0;
		public int eModifier = 0;
		public int sModifier = 0;
		public int wModifier = 0;
		public int cModifier = 0;
		public int aModifier = 0;

		public int nDivisor = 1;
		public int eDivisor = 1;
		public int sDivisor = 1;
		public int wDivisor = 1;
		public int cDivisor = 1;
		public int aDivisor = 1;

		public int nMultiplier = 0;
		public int eMultiplier = 0;
		public int sMultiplier = 0;
		public int wMultiplier = 0;
		public int cMultiplier = 0;
		public int aMultiplier = 0;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			n ??= new EffectX() { multiplier = nMultiplier, modifier = nModifier, divisor = nDivisor };
			e ??= new EffectX() { multiplier = eMultiplier, modifier = eModifier, divisor = eDivisor };
			s ??= new EffectX() { multiplier = sMultiplier, modifier = sModifier, divisor = sDivisor };
			w ??= new EffectX() { multiplier = wMultiplier, modifier = wModifier, divisor = wDivisor };
			c ??= new EffectX() { multiplier = cMultiplier, modifier = cModifier, divisor = cDivisor };
			a ??= new EffectX() { multiplier = aMultiplier, modifier = aModifier, divisor = aDivisor };

			base.Initialize(eff, subeffIndex);
		}
	}
}