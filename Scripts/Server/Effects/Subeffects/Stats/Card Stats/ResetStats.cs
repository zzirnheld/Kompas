using System.Threading.Tasks;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ResetStatsData : SubeffectData
{
	[JsonProperty]
	public bool resetN = false;
	[JsonProperty]
	public bool resetE = false;
	[JsonProperty]
	public bool resetS = false;
	[JsonProperty]
	public bool resetW = false;
	[JsonProperty]
	public bool resetC = false;
	[JsonProperty]
	public bool resetA = false;
}

public class ResetStats : ServerSubeffect
{
	private readonly bool resetN;
	private readonly bool resetE;
	private readonly bool resetS;
	private readonly bool resetW;
	private readonly bool resetC;
	private readonly bool resetA;

	public ResetStats(ResetStatsData data) : base(data)
	{
		resetN = data.resetN;
		resetE = data.resetE;
		resetS = data.resetS;
		resetW = data.resetW;
		resetC = data.resetC;
		resetA = data.resetA;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var card = GetCardTarget(resolution.Context) ?? throw new NullCardException(TargetWasNull);
		if (resetN) card.SetN(card.BaseN, Effect);
		if (resetE) card.SetE(card.BaseE, Effect);
		if (resetS) card.SetS(card.BaseS, Effect);
		if (resetW) card.SetW(card.BaseW, Effect);
		if (resetC) card.SetC(card.BaseC, Effect);
		if (resetA) card.SetA(card.BaseA, Effect);

		return Task.FromResult(ResolutionInfo.Next);
	}
}