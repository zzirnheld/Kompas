using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;
using Kompas.Effects.Models.TriggeringEvent;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging;

public class ChangeCardStatsData : HangingEffectData
{
	[JsonProperty]
	public int nModifier = 0;
	[JsonProperty]
	public int eModifier = 0;
	[JsonProperty]
	public int sModifier = 0;
	[JsonProperty]
	public int wModifier = 0;
	[JsonProperty]
	public int cModifier = 0;
	[JsonProperty]
	public int aModifier = 0;

	[JsonProperty]
	public int nDivisor = 1;
	[JsonProperty]
	public int eDivisor = 1;
	[JsonProperty]
	public int sDivisor = 1;
	[JsonProperty]
	public int wDivisor = 1;
	[JsonProperty]
	public int cDivisor = 1;
	[JsonProperty]
	public int aDivisor = 1;

	[JsonProperty]
	public int nMultiplier = 0;
	[JsonProperty]
	public int eMultiplier = 0;
	[JsonProperty]
	public int sMultiplier = 0;
	[JsonProperty]
	public int wMultiplier = 0;
	[JsonProperty]
	public int cMultiplier = 0;
	[JsonProperty]
	public int aMultiplier = 0;
}

public class ChangeCardStats : ChangeCardStats<ChangeCardStatsData>
{
	public ChangeCardStats(ChangeCardStatsData data) : base(data) { }
}

public abstract class ChangeCardStats<DataType> : HangingEffectSubeffect<DataType>
	where DataType : ChangeCardStatsData
{
	private readonly int nModifier;
	private readonly int eModifier;
	private readonly int sModifier;
	private readonly int wModifier;
	private readonly int cModifier;
	private readonly int aModifier;

	private readonly int nDivisor;
	private readonly int eDivisor;
	private readonly int sDivisor;
	private readonly int wDivisor;
	private readonly int cDivisor;
	private readonly int aDivisor;

	private readonly int nMultiplier;
	private readonly int eMultiplier;
	private readonly int sMultiplier;
	private readonly int wMultiplier;
	private readonly int cMultiplier;
	private readonly int aMultiplier;

	public ChangeCardStats(DataType data) : base(data)
	{
		nModifier = data.nModifier;
		eModifier = data.eModifier;
		sModifier = data.sModifier;
		wModifier = data.wModifier;
		cModifier = data.cModifier;
		aModifier = data.aModifier;

		nMultiplier = data.nMultiplier;
		eMultiplier = data.eMultiplier;
		sMultiplier = data.sMultiplier;
		wMultiplier = data.wMultiplier;
		cMultiplier = data.cMultiplier;
		aMultiplier = data.aMultiplier;

		nDivisor = data.nDivisor;
		eDivisor = data.eDivisor;
		sDivisor = data.sDivisor;
		wDivisor = data.wDivisor;
		cDivisor = data.cDivisor;
		aDivisor = data.aDivisor;
	}

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