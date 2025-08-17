using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTriggeringCardData : SubeffectData
{
	[JsonProperty]
	public bool contextSecondaryCard = false;
	[JsonProperty]
	public bool info = false;
	[JsonProperty]
	public bool cause = false;
}

public class TargetTriggeringCard : ServerSubeffect
{
	private readonly bool contextSecondaryCard;
	private readonly bool info;
	private readonly bool cause;

	public TargetTriggeringCard(TargetTriggeringCardData data) : base(data)
	{
		contextSecondaryCard = data.contextSecondaryCard;
		info = data.info;
		cause = data.cause;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var cardInfoToTarget = resolution.Context.TriggerContext?.MainCardBefore;
		if (contextSecondaryCard) cardInfoToTarget = resolution.Context.TriggerContext?.SecondaryCardBefore;
		if (cause) cardInfoToTarget = resolution.Context.TriggerContext?.CauseCardBefore;

		if (cardInfoToTarget == null)
			throw new NullCardException(debugMessage: $"Trigger context was {resolution.Context.TriggerContext}",
				message: NoValidCardTarget);

		if (info) resolution.Context.CardInfoTargets.Add(cardInfoToTarget);
		else resolution.AddTarget(cardInfoToTarget.Card);

		return Task.FromResult(ResolutionInfo.Next);
	}
}