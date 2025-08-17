using System;
using System.Linq;
using System.Threading.Tasks;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Cards;
using Kompas.Effects.Models.Identities.ManyCards;
using Kompas.Effects.Models.Identities.Numbers;
using Kompas.Effects.Subeffects;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SetCardStatsData : CardStatChangeDataBase
{
	//IIdentities take precendence over the "val" variables from legacy cards
	[JsonProperty]
	public int nVal = -1;
	[JsonProperty]
	public int eVal = -1;
	[JsonProperty]
	public int sVal = -1;
	[JsonProperty]
	public int wVal = -1;
	[JsonProperty]
	public int cVal = -1;
	[JsonProperty]
	public int aVal = -1;
}

public class SetCardStats : CardStatChangeBase
{
	protected readonly IIdentity<int>? n;
	protected readonly IIdentity<int>? e;
	protected readonly IIdentity<int>? s;
	protected readonly IIdentity<int>? w;
	protected readonly IIdentity<int>? c;
	protected readonly IIdentity<int>? a;

	public SetCardStats(SetCardStatsData data) : base(data)
	{
		n = data.n ?? FromVal(data.nVal);
		e = data.e ?? FromVal(data.eVal);
		s = data.s ?? FromVal(data.sVal);
		w = data.w ?? FromVal(data.wVal);
		c = data.c ?? FromVal(data.cVal);
		a = data.a ?? FromVal(data.aVal);
	}

	/// <returns>
	/// A <see cref="Constant"/> of <paramref name="val"/> if val GTE 0,
	/// or null if val LT 0
	/// </returns>
	private static IIdentity<int>? FromVal(int val)
		=> val >= 0
			? new Constant() { constant = val }
			: null;

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);

		var initContext = DefaultInitializationContext;
		n?.Initialize(initContext);
		e?.Initialize(initContext);
		s?.Initialize(initContext);
		w?.Initialize(initContext);
		c?.Initialize(initContext);
		a?.Initialize(initContext);
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		// If any of the stats are absent, don't set them.
		int? nValue = n?.From(resolution.Context, resolution.Context);
		int? eValue = e?.From(resolution.Context, resolution.Context);
		int? sValue = s?.From(resolution.Context, resolution.Context);
		int? wValue = w?.From(resolution.Context, resolution.Context);
		int? cValue = c?.From(resolution.Context, resolution.Context);
		int? aValue = a?.From(resolution.Context, resolution.Context);

		int? turnsOnBoardChange = turnsOnBoard?.From(resolution.Context, resolution.Context);
		int? attacksThisTurnChange = attacksThisTurn?.From(resolution.Context, resolution.Context);
		int? spacesMovedChange = spacesMoved?.From(resolution.Context, resolution.Context);
		int? durationChange = duration?.From(resolution.Context, resolution.Context);

		foreach (var card in GetCardsToAffect(resolution.Context))
		{
			ValidateCardOnBoard(card);

			card.SetStats(card.Stats.ReplaceWith((nValue, eValue, sValue, wValue, cValue, aValue)), Effect);

			if (turnsOnBoardChange.HasValue) card.TurnsOnBoard = turnsOnBoardChange.Value;
			if (attacksThisTurnChange.HasValue) card.AttacksThisTurn = attacksThisTurnChange.Value;
			if (spacesMovedChange.HasValue) card.SpacesMoved = spacesMovedChange.Value;
			if (durationChange.HasValue) card.Duration = durationChange.Value;
		}

		return Task.FromResult(ResolutionInfo.Next);
	}
}