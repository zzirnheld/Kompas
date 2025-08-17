using System;
using System.Linq;
using System.Threading.Tasks;
using Kompas.Effects.Models.Identities.Numbers;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SetCardStatsData : ChangeCardStatsDataBase
{
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

public class SetCardStats : ChangeCardStatsBase
{
	public SetCardStats(SetCardStatsData data) : base(PopulateIdentities(data)) { }

	private static ChangeCardStatsDataBase PopulateIdentities(SetCardStatsData data)
	{
		if (data.nVal >= 0) data.n ??= new Constant() { constant = data.nVal };
		if (data.eVal >= 0) data.e ??= new Constant() { constant = data.eVal };
		if (data.sVal >= 0) data.s ??= new Constant() { constant = data.sVal };
		if (data.wVal >= 0) data.w ??= new Constant() { constant = data.wVal };
		if (data.cVal >= 0) data.c ??= new Constant() { constant = data.cVal };
		if (data.aVal >= 0) data.a ??= new Constant() { constant = data.aVal };
		return data;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var cards = this.cards.From(resolution.Context, resolution.Context)
			?? throw new InvalidOperationException();

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

		foreach (var card in cards.Select(c => c.Card))
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