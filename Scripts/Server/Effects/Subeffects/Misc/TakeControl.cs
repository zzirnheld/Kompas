using System.Threading.Tasks;
using Kompas.Gamestate.Players;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TakeControl : ServerSubeffect
{
	public int ControllerIndexOffset = 0;

    //TODO abstract this logic into a parent class with other player offset things
    private IPlayer GetNewController(IServerResolutionContext context)
    {
        return Game.Players[(GetPlayerTarget(context).Index + ControllerIndexOffset) % Game.Players.Length];
    }

    public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
        GetCardTarget(resolution.Context).ControllingPlayer = GetNewController(resolution.Context);
		return Task.FromResult(ResolutionInfo.Next);
	}
}