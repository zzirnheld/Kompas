using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SetCardStats : ChangeCardStatsBase
{

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		int? nValue = n?.From(resolution.Context, resolution.Context);
		int? eValue = e?.From(resolution.Context, resolution.Context);
		int? sValue = s?.From(resolution.Context, resolution.Context);
		int? wValue = w?.From(resolution.Context, resolution.Context);
		int? cValue = c?.From(resolution.Context, resolution.Context);
		int? aValue = a?.From(resolution.Context, resolution.Context);

		int? turnsOnBoardChange	 = turnsOnBoard?.From(resolution.Context, resolution.Context);
		int? attacksThisTurnChange  = attacksThisTurn?.From(resolution.Context, resolution.Context);
		int? spacesMovedChange	  = spacesMoved?.From(resolution.Context, resolution.Context);
		int? durationChange		 = duration?.From(resolution.Context, resolution.Context);

		var cards = this.cards.From(resolution.Context, resolution.Context)
			?? throw new InvalidOperationException();
		foreach (var card in cards.Select(c => c.Card))
		{
			ValidateCardOnBoard(card);

			card.SetStats(card.Stats.ReplaceWith((nValue, eValue, sValue, wValue, cValue, aValue)), Effect);

			if (turnsOnBoardChange.HasValue)	card.TurnsOnBoard	   = turnsOnBoardChange.Value;
			if (attacksThisTurnChange.HasValue) card.AttacksThisTurn	= attacksThisTurnChange.Value;
			if (spacesMovedChange.HasValue)	 card.SpacesMoved		= spacesMovedChange.Value;
			if (durationChange.HasValue)		card.Duration		   = durationChange.Value;
		}

		return Task.FromResult(ResolutionInfo.Next);
	}
}