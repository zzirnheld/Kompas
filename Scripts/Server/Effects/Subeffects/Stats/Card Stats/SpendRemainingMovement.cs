using System.Threading.Tasks;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SpendRemainingMovementData : SubeffectData
{
	[JsonProperty]
	public int mult = 1;
	[JsonProperty]
	public int div = 1;
	[JsonProperty]
	public int mod = 0;
}

public class SpendRemainingMovement : ServerSubeffect
{
	private readonly int mult;
	private readonly int div;
	private readonly int mod;

	public SpendRemainingMovement(SpendRemainingMovementData data) : base(data)
	{
		mult = data.mult;
		div = data.div;
		mod = data.mod;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var card = GetCardTarget(resolution.Context) ?? throw new NullCardException(TargetWasNull);
		int toSpend = (card.SpacesCanMove * mult / div) + mod;
		if (toSpend <= 0 || card.SpacesCanMove < toSpend) return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));

		card.SpacesMoved += toSpend;
		return Task.FromResult(ResolutionInfo.Next);
	}
}