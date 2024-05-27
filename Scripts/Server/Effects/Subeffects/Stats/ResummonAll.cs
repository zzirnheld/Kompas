using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models.Restrictions.Gamestate;
using Kompas.Server.Effects.Controllers;
using Newtonsoft.Json;
using System.Linq;
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
			//TODO make a toManyContexts that becomes a wrapper object for an enumerable set of contexts, which can all Capture the same event
			foreach (var c in Game.Board.Cards.Where(c => cardRestriction.IsValid(c, ResolutionContext)))
			{
				var contexts = IEventContext.Build(Trigger.Play)
					.PrimarilyAffecting(c)
					.CausedBy(Effect)
					.ForPlayer(PlayerTarget)
					.At(c.Position)
					.Capture(() => { },
						ctxt => ctxt,
						ctxt => ctxt.CloneForEvent(Trigger.Arrive));
				ServerGame.StackController.TriggerFor(contexts);
			}

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}