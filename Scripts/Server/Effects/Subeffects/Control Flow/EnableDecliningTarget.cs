using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Server.Networking;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class EnableDecliningTargetData : SubeffectData { }

public class EnableDecliningTarget : ServerSubeffect<EnableDecliningTargetData>
{
	public EnableDecliningTarget(EnableDecliningTargetData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var player = GetPlayerTarget(resolution.Context) ?? throw new NullPlayerException(TargetWasNull);

		ServerNotifier.EnableDecliningTarget(player);
		resolution.Context.CanDeclineTarget = true;

		return Task.FromResult(ResolutionInfo.Next);
	}
}