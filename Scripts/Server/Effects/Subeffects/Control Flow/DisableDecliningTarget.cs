using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Server.Networking;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class DisableDecliningTargetData : SubeffectData { }

public class DisableDecliningTarget : ServerSubeffect<DisableDecliningTargetData>
{
	public DisableDecliningTarget(DisableDecliningTargetData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var player = GetPlayerTarget(resolution.Context) ?? throw new NullPlayerException(TargetWasNull);
		ServerNotifier.DisableDecliningTarget(player);
		resolution.Context.CanDeclineTarget = false;

		return Task.FromResult(ResolutionInfo.Next);
	}
}