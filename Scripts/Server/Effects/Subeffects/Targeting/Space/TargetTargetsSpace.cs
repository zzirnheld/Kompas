using System.Threading.Tasks;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetTargetsSpaceData : SubeffectData { }

public class TargetTargetsSpace : ServerSubeffect
{
	public TargetTargetsSpace(TargetTargetsSpaceData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var cardTarget = GetCardTarget(resolution.Context);
		if (cardTarget.Location != Location.Board)
			return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
		if (cardTarget.Position == null)
			throw new NullSpaceOnBoardException(cardTarget);

		resolution.AddSpace(cardTarget.Position.Copy);
		return Task.FromResult(ResolutionInfo.Next);
	}
}