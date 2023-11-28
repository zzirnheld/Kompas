using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Server.Gamestate;
using System.Collections.Generic;
using Godot;
using Kompas.Effects.Models.Restrictions;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging
{
	public class ChangeCardStats : HangingEffectSubeffect
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

		protected CardStats Buff
		{
			get
			{
				CardStats buff = (nMultiplier, eMultiplier, sMultiplier, wMultiplier, cMultiplier, aMultiplier);
				buff *= Effect.X;
				buff += (nModifier, eModifier, sModifier, wModifier, cModifier, aModifier);
				buff /= (nDivisor, eDivisor, sDivisor, wDivisor, cDivisor, aDivisor);
				return buff;
			}
		}

		protected override IEnumerable<HangingEffect> CreateHangingEffects()
		{
			if (CardTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && CardTarget.Location != Location.Board)
				throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

			GD.Print($"Creating temp NESW buff effect during context {ResolutionContext}");

			var temp = new ChangeCardStatsEffect(end: End, fallOff: FallOff,
				sourceEff: ServerEffect, currentContext: ResolutionContext,
				buffRecipient: CardTarget, buff: Buff);

			return new List<HangingEffect>() { temp };
		}

		protected class ChangeCardStatsEffect : HangingEffect
		{
			private readonly GameCard buffRecipient;
			private readonly CardStats buff;

			public ChangeCardStatsEffect(EndCondition end, EndCondition fallOff, ServerEffect sourceEff,
				IResolutionContext currentContext, GameCard buffRecipient, CardStats buff)
				: base(end, fallOff, sourceEff, currentContext, removeIfEnd: true)
			{
				this.buffRecipient = buffRecipient ?? throw new System.ArgumentNullException(nameof(buffRecipient), "Null characcter card in temporary nesw buff");
				this.buff = buff;

				buffRecipient.AddToStats(buff, stackSrc: sourceEff);
			}

			protected override void ResolveLogic(TriggeringEventContext context)
			{
				try
				{
					buffRecipient.AddToStats(-1 * buff, stackSrc: Effect);
				}
				catch (CardNotHereException) { }
			}
		}
	}
}