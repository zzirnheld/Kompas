using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ShowData : SubeffectData { }

/// <summary>
/// Like a RevealSubeffect, but isn't impossible if the card is already revealed.
/// </summary>
public class Show : ServerSubeffect
{
	public Show(ShowData data) : base(data) { }

	public override bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null)
		=> context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext)) == null;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var card = GetCardTarget(resolution.Context);
		try { card.Reveal(Effect); }
		catch (AlreadyKnownException) { }

		return Task.FromResult(ResolutionInfo.Next);
	}
}