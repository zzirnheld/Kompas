using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class AttachData : SubeffectData
{
	/// <summary>
	/// the index for the card to be attached to.
	/// default is two targets ago
	/// </summary>
	[JsonProperty]
	public int targetToAttachTo = -2;
}

public class Attach : ServerSubeffect
{
	private readonly int targetToAttachTo;

	public Attach(AttachData data) : base(data)
	{
		targetToAttachTo = data.targetToAttachTo;
	}

	public override bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null)
		=> context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext)) == null
		|| context.GetCardTarget(targetToAttachTo) == null;

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var toAttach = GetCardTarget(resolution.Context);
		var attachTo = resolution.Context.GetCardTarget(targetToAttachTo)
			?? throw new NullCardException(TargetWasNull);

		attachTo.AddAugment(toAttach, stackSrc: Effect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}