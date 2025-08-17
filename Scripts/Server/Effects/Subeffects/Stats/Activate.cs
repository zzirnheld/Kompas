using System.Threading.Tasks;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ActivateData : SubeffectData
{
	[JsonProperty]
	public bool activate = true;
}

public class Activate : ServerSubeffect
{
	private readonly bool activate;

	public Activate(ActivateData data) : base(data)
	{
		activate = data.activate;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var card = GetCardTarget(resolution.Context);
		card.SetActivated(activate, Effect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}