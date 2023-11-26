using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;
using Godot;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetTriggeringCard : ServerSubeffect
	{
		public bool contextSecondaryCard = false;
		public bool info = false;
		public bool cause = false;

		public override Task<ResolutionInfo> Resolve()
		{
			var cardInfoToTarget = ResolutionContext.TriggerContext?.mainCardInfoBefore;
			if (contextSecondaryCard) cardInfoToTarget = ResolutionContext.TriggerContext?.secondaryCardInfoBefore;
			if (cause) cardInfoToTarget = ResolutionContext.TriggerContext?.cardCauseBefore;

			if (cardInfoToTarget == null)
				throw new NullCardException(debugMessage: $"Trigger context was {ResolutionContext.TriggerContext}", 
					message: NoValidCardTarget);

			if (info) ServerEffect.CardInfoTargets.Add(cardInfoToTarget);
			else ServerEffect.AddTarget(cardInfoToTarget.Card);

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}