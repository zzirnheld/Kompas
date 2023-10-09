using KompasCore.Cards.Movement;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class Move : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null) throw new NullCardException(TargetWasNull);

			CardTarget.Move(SpaceTarget, false, Effect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}