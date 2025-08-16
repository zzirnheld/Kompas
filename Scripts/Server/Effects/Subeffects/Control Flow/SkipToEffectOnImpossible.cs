using System.Threading.Tasks;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SkipToEffectOnImpossibleData : SubeffectData { }

/// <summary>
/// Resolves a specified subeffect if at any point the effect is declared impossible
/// </summary>
public class SkipToEffectOnImpossible : ServerSubeffect
{
	public SkipToEffectOnImpossible(SkipToEffectOnImpossibleData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var currentResolution = resolution.Context
			?? throw new EffectNotResolvingException(ServerEffect);
		currentResolution.OnImpossible = this;
		return Task.FromResult(ResolutionInfo.Next);
	}

	public override Task<ResolutionInfo> OnImpossible(ServerEffectResolution resolution, string why)
	{
		//forget about this effect on impossible, and jump to a new one
		var currentResolution = resolution.Context
			?? throw new EffectNotResolvingException(ServerEffect);
		currentResolution.OnImpossible = null;
		return Task.FromResult(ResolutionInfo.Index(JumpIndex));
	}
}
