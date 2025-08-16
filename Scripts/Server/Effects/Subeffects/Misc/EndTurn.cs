using System.Threading.Tasks;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public class EndTurnData : SubeffectData { }

public class EndTurn : ServerSubeffect
{
	public EndTurn(EndTurnData data) : base(data) { }

	public override async Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		await ServerGame.SwitchTurn();
		return ResolutionInfo.Next;
	}
}