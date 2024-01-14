using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Attach : ServerSubeffect
	{
		//the index for the card to be attached to.
		//default is two targets ago
		public int targetToAttachTo = -2;

		public override bool IsImpossible (TargetingContext? overrideContext = null)
			=> GetCardTarget(overrideContext) == null
			|| Effect.GetTarget(targetToAttachTo) == null;

		public override Task<ResolutionInfo> Resolve()
		{
			var toAttach = CardTarget;
			var attachTo = Effect.GetTarget(targetToAttachTo);

			//if everything goes to plan, resolve the next subeffect
			if (toAttach == null) throw new NullCardException(TargetWasNull);
			else if (attachTo == null) throw new NullCardException(TargetWasNull);

			attachTo.AddAugment(toAttach, stackSrc: Effect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}