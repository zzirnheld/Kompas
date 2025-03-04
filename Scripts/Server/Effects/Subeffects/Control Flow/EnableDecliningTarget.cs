using Kompas.Gamestate.Exceptions;
using Kompas.Server.Networking;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class EnableDecliningTarget : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve()
	{
		var player = PlayerTarget ?? throw new NullPlayerException(TargetWasNull);
		
		ServerNotifier.EnableDecliningTarget(player);
		ResolutionContext.CanDeclineTarget = true;

		return Task.FromResult(ResolutionInfo.Next);
	}
}