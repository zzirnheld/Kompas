using Kompas.Effects.Models;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Resummon : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && CardTarget.Location != Location.Board)
				throw new InvalidLocationException(CardTarget.Location, CardTarget, "Target not on board :(");

			var ctxt = new TriggeringEventContext(game: ServerGame, cardBefore: CardTarget, 
				stackableCause: Effect, player: PlayerTarget, space: CardTarget.Position);
			ctxt.CacheCardInfoAfter();
			ServerEffect.EffectsController.TriggerForCondition(Trigger.Play, ctxt);
			ServerEffect.EffectsController.TriggerForCondition(Trigger.Arrive, ctxt);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}