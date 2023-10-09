using System.Linq;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class TargetAll: CardTarget
	{
		public override bool IsImpossible(TargetingContext overrideContext = null)
			=> !DeterminePossibleTargets().Any();

		public override Task<ResolutionInfo> Resolve()
		{
			//check what targets there are now, before you add them, to not mess with NotAlreadyTarget restriction
			//because Linq executes lazily, it would otherwise add the targets, then re-execute the query and not find any
			var targets = DeterminePossibleTargets();
			foreach (var t in targets) Effect.AddTarget(t);

			if (targets.Any()) return Task.FromResult(ResolutionInfo.Next);
			else return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
		}
	}

}