using KompasCore.Cards;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions.GamestateRestrictionElements;
using KompasCore.GameCore;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class ResummonAll : ServerSubeffect
	{
		public IRestriction<IGameCard> cardRestriction;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			cardRestriction ??= new AlwaysValid();
			cardRestriction.Initialize(DefaultInitializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction?.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			foreach (var c in Game.BoardController.CardsWhere(c => cardRestriction.IsValid(c, ResolutionContext)))
			{
				var ctxt = new TriggeringEventContext(game: ServerGame, CardBefore: c, stackableCause: Effect, player: EffectController, space: c.Position);
				ctxt.CacheCardInfoAfter();
				ServerEffect.EffectsController.TriggerForCondition(Trigger.Play, ctxt);
				ServerEffect.EffectsController.TriggerForCondition(Trigger.Arrive, ctxt);
			}

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}