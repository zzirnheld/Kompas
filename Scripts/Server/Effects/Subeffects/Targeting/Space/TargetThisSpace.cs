using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetThisSpace : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			_ = Effect ?? throw new System.NullReferenceException("Was the effect not resolving?");

			if (Effect.Card?.Location != Location.Board)
				return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

			ServerEffect.AddSpace(Effect.Card.Position ?? throw new NullSpaceOnBoardException(Effect.Card));
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}