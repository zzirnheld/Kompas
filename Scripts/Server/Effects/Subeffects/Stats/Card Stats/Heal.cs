using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Heal : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && CardTarget.Location != Location.Board)
				throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);
			else if (CardTarget.E >= CardTarget.BaseE)
				throw new InvalidCardException(CardTarget, TooMuchEForHeal);

			int healedFor = CardTarget.BaseE - CardTarget.E;
			CardTarget.SetE(CardTarget.BaseE, stackSrc: ServerEffect);
			ServerEffect.EffectsController.TriggerForCondition(Trigger.Healed, new TriggeringEventContext(Game, CardBefore: CardTarget, stackableCause: Effect, player: PlayerTarget, x: healedFor));
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}