using Kompas.Gamestate.Exceptions;
using Kompas.Server.Networking;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class EnableDecliningTarget : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var player = GetPlayerTarget(resolution.Context) ?? throw new NullPlayerException(TargetWasNull);

		ServerNotifier.EnableDecliningTarget(player);
		resolution.Context.CanDeclineTarget = true;

		return Task.FromResult(ResolutionInfo.Next);
	}
}