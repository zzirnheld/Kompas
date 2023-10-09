using Kompas.Effects.Models;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class Heal : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && CardTarget.Location != CardLocation.Board)
				throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);
			else if (CardTarget.E >= CardTarget.BaseE)
				throw new InvalidCardException(CardTarget, TooMuchEForHeal);

			int healedFor = CardTarget.BaseE - CardTarget.E;
			CardTarget.SetE(CardTarget.BaseE, stackSrc: ServerEffect);
			ServerEffect.EffectsController.TriggerForCondition(Trigger.Healed, new TriggeringEventContext(Game, CardBefore: CardTarget, stackableCause: Effect, player: EffectController, x: healedFor));
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}