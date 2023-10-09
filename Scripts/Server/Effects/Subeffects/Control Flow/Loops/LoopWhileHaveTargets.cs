using System.Linq;
using Kompas.Effects.Models.Identities;

namespace Kompas.Server.Effects.Subeffects
{
	public class LoopWhileHaveTargets : Loop
	{
		public bool delete = false;

		public int remainingTargets = 0;
		public IIdentity<int> leaveRemainingTargets;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			leaveRemainingTargets ??= new Kompas.Effects.Models.Identities.Numbers.Constant() { constant = remainingTargets };
		}

		protected override bool ShouldContinueLoop
		{
			get
			{
				//if we're deleting and there's something to delete, delete it.
				if (delete && ServerEffect.CardTargets.Any()) RemoveTarget();
				//after any delete that might have happened, check if there's still targets
				return ServerEffect.CardTargets.Count() > leaveRemainingTargets.From(ResolutionContext);
			}
		}
	}
}