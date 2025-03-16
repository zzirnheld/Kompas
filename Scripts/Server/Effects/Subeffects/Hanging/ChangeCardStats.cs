using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;
using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging;

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

    protected CardStats ComputeBuff(int x)
    {
        CardStats buff = (nMultiplier, eMultiplier, sMultiplier, wMultiplier, cMultiplier, aMultiplier);
        buff *= x;
        buff += (nModifier, eModifier, sModifier, wModifier, cModifier, aModifier);
        buff /= (nDivisor, eDivisor, sDivisor, wDivisor, cDivisor, aDivisor);
        return buff;
    }

    protected override IEnumerable<HangingEffect> CreateHangingEffects(IServerResolutionContext context)
	{
		if (GetCardTarget(context) == null)
			throw new NullCardException(TargetWasNull);
		else if (forbidNotBoard && GetCardTarget(context).Location != Location.Board)
			throw new InvalidLocationException(GetCardTarget(context).Location, GetCardTarget(context), ChangedStatsOfCardOffBoard);

		Logger.Log($"Creating temp NESW buff effect during context {context}");

		var temp = new ChangeCardStatsEffect(end: End, fallOff: FallOff,
			sourceEff: ServerEffect, currentContext: context,
			buffRecipient: GetCardTarget(context), buff: ComputeBuff(context.X));

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

		protected override void ResolveLogic(IEventContext context)
		{
			try
			{
				buffRecipient.AddToStats(-1 * buff, stackSrc: Effect);
			}
			catch (CardNotHereException) { }
		}
	}
}