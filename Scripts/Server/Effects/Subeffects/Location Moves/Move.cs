using Kompas.Cards.Movement;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Move : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null) throw new NullCardException(TargetWasNull);

			CardTarget.Move(SpaceTarget, false, PlayerTarget, Effect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}