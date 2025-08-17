using System.Threading.Tasks;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class NegateData : SubeffectData
{
	[JsonProperty]
	public bool negated = true;
}

public class Negate : ServerSubeffect
{
	private readonly bool negated;

	public Negate(NegateData data) : base(data)
	{
		negated = data.negated;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var card = GetCardTarget(resolution.Context) ?? throw new NullCardException(TargetWasNull);
		card.SetNegated(negated, ServerEffect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}