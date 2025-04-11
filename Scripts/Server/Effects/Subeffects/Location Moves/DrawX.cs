using System.Threading.Tasks;
using Kompas.Effects.Models;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class DrawX : ServerSubeffect
{
    protected virtual int GetToDraw(IResolutionContext context) => AdjustX(context);

    [JsonProperty]
	public bool addAsTarget = false;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var drawn = ServerGame.DrawX(GetPlayerTarget(resolution.Context), GetToDraw(resolution.Context), Effect);
		if (addAsTarget) foreach (var card in drawn) resolution.AddTarget(card);

		if (drawn.Count < GetToDraw(resolution.Context)) return Task.FromResult(ResolutionInfo.Impossible(CouldntDrawAllX));
		else return Task.FromResult(ResolutionInfo.Next);
	}
}