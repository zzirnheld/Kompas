using System.Threading.Tasks;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects;

/// <summary>
/// Removes any effect currently set to trigger if an effect is declared impossible.
/// </summary>
public class ClearOnImpossible : ServerSubeffect
{
	public ClearOnImpossible(ClearOnImpossibleData data) : base(data) { }

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var currentResolution = resolution.Context
			?? throw new EffectNotResolvingException(ServerEffect);
		currentResolution.OnImpossible = null;
		return Task.FromResult(ResolutionInfo.Next);
	}
}

public class ClearOnImpossibleData : SubeffectData { }