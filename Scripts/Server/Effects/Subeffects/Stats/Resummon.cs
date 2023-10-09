using Kompas.Effects.Models;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class Resummon : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && CardTarget.Location != CardLocation.Board)
				throw new InvalidLocationException(CardTarget.Location, CardTarget, "Target not on board :(");

			var ctxt = new TriggeringEventContext(game: ServerGame, CardBefore: CardTarget, 
				stackableCause: Effect, player: EffectController, space: CardTarget.Position);
			ctxt.CacheCardInfoAfter();
			ServerEffect.EffectsController.TriggerForCondition(Trigger.Play, ctxt);
			ServerEffect.EffectsController.TriggerForCondition(Trigger.Arrive, ctxt);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}