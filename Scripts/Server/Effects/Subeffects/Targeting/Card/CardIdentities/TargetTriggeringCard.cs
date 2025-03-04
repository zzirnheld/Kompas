using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTriggeringCard : ServerSubeffect
{
	public bool contextSecondaryCard = false;
	public bool info = false;
	public bool cause = false;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var cardInfoToTarget = resolution.Context.TriggerContext?.MainCardBefore;
		if (contextSecondaryCard) cardInfoToTarget = resolution.Context.TriggerContext?.SecondaryCardBefore;
		if (cause) cardInfoToTarget = resolution.Context.TriggerContext?.CauseCardBefore;

		if (cardInfoToTarget == null)
			throw new NullCardException(debugMessage: $"Trigger context was {resolution.Context.TriggerContext}", 
				message: NoValidCardTarget);

		if (info) ServerEffect.CardInfoTargets.Add(cardInfoToTarget);
		else resolution.AddTarget(cardInfoToTarget.Card);

		return Task.FromResult(ResolutionInfo.Next);
	}
}