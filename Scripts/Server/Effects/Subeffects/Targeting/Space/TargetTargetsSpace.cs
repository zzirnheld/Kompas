using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetTargetsSpace : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget?.Location != Location.Board)
				return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

			if (CardTarget.Position == null) throw new NullSpaceOnBoardException(CardTarget);

			Effect.AddSpace(CardTarget.Position.Copy);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}