using Kompas.Cards.Models;
using Kompas.Gamestate.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class SetCardStats : ChangeCardStatsBase
	{

		public override Task<ResolutionInfo> Resolve()
		{
			int? nValue = n?.From(ResolutionContext, ResolutionContext);
			int? eValue = e?.From(ResolutionContext, ResolutionContext);
			int? sValue = s?.From(ResolutionContext, ResolutionContext);
			int? wValue = w?.From(ResolutionContext, ResolutionContext);
			int? cValue = c?.From(ResolutionContext, ResolutionContext);
			int? aValue = a?.From(ResolutionContext, ResolutionContext);

			int? turnsOnBoardChange		= turnsOnBoard?.From(ResolutionContext, ResolutionContext);
			int? attacksThisTurnChange	= attacksThisTurn?.From(ResolutionContext, ResolutionContext);
			int? spacesMovedChange		= spacesMoved?.From(ResolutionContext, ResolutionContext);
			int? durationChange		 	= duration?.From(ResolutionContext, ResolutionContext);

			var cards = this.cards.From(ResolutionContext, ResolutionContext)
				?? throw new InvalidOperationException();
			foreach (var card in cards.Select(c => c.Card))
			{
				ValidateCardOnBoard(card);

				card.SetStats(CardStats.Of(card).ReplaceWith((nValue, eValue, sValue, wValue, cValue, aValue)), Effect);

				if (turnsOnBoardChange.HasValue)	card.TurnsOnBoard	 = turnsOnBoardChange.Value;
				if (attacksThisTurnChange.HasValue) card.AttacksThisTurn = attacksThisTurnChange.Value;
				if (spacesMovedChange.HasValue)	 	card.SpacesMoved	 = spacesMovedChange.Value;
				if (durationChange.HasValue)		card.Duration		 = durationChange.Value;
			}

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}