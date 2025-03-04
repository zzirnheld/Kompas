using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class EndTurn : ServerSubeffect
{
	public override async Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		await ServerGame.SwitchTurn();
		return ResolutionInfo.Next;
	}
}