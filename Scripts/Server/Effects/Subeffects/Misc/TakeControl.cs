using System.Threading.Tasks;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TakeControlData : SubeffectData
{
    [JsonProperty]
    public int controllerIndexOffset = 0;
}

public class TakeControl : ServerSubeffect
{
    private readonly int controllerIndexOffset;

    public TakeControl(TakeControlData data) : base(data)
    {
        controllerIndexOffset = data.controllerIndexOffset;
	}

	//TODO abstract this logic into a parent class with other player offset things
	private IPlayer GetNewController(IServerResolutionContext context)
    {
        return Game.Players[(GetPlayerTarget(context).Index + controllerIndexOffset) % Game.Players.Length];
    }

    public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
    {
        GetCardTarget(resolution.Context).ControllingPlayer = GetNewController(resolution.Context);
        return Task.FromResult(ResolutionInfo.Next);
    }
}