using Kompas.Gamestate.Exceptions;
using Kompas.Server.Networking;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class DisableDecliningTarget : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var player = PlayerTarget ?? throw new NullPlayerException(TargetWasNull);
		ServerNotifier.DisableDecliningTarget(player);
		resolution.Context.CanDeclineTarget = false;

		return Task.FromResult(ResolutionInfo.Next);
	}
}