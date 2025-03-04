using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects;

/// <summary>
/// Resolves a specified subeffect if at any point the effect is declared impossible
/// </summary>
public class SkipToEffectOnImpossible : ServerSubeffect
{
	public override Task<ResolutionInfo> Resolve()
	{
		var currentResolution = ServerEffect.CurrentServerResolutionContext
			?? throw new EffectNotResolvingException(ServerEffect);
		currentResolution.OnImpossible = this;
		return Task.FromResult(ResolutionInfo.Next);
	}

	public override Task<ResolutionInfo> OnImpossible(string why)
	{
		//forget about this effect on impossible, and jump to a new one
		var currentResolution = ServerEffect.CurrentServerResolutionContext
			?? throw new EffectNotResolvingException(ServerEffect);
		currentResolution.OnImpossible = null;
		return Task.FromResult(ResolutionInfo.Index(JumpIndex));
	}
}
