using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models.Restrictions.Gamestate;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class ResummonAll : ServerSubeffect
	{
		[JsonProperty]
		public IRestriction<IGameCardInfo> cardRestriction = new AlwaysValid();

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			cardRestriction.Initialize(DefaultInitializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction?.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			foreach (var c in Game.Board.CardsWhere(c => cardRestriction.IsValid(c, ResolutionContext)))
			{
				var ctxt = new TriggeringEventContext(game: ServerGame, CardBefore: c, stackableCause: Effect, player: PlayerTarget, space: c.Position);
				ctxt.CacheCardInfoAfter();
				ServerEffect.EffectsController.TriggerForCondition(Trigger.Play, ctxt);
				ServerEffect.EffectsController.TriggerForCondition(Trigger.Arrive, ctxt);
			}

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}