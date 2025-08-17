using System.Threading.Tasks;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetThisSpaceData : SubeffectData { }

public class TargetThisSpace : ServerSubeffect
{
	public TargetThisSpace(TargetThisSpaceData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		_ = Effect ?? throw new System.NullReferenceException("Was the effect not resolving?");

		if (Effect.Card?.Location != Location.Board)
			return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

		resolution.AddSpace(Effect.Card.Position ?? throw new NullSpaceOnBoardException(Effect.Card));
		return Task.FromResult(ResolutionInfo.Next);
	}
}