using Kompas.Gamestate.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class SetCardStats : ChangeCardStatsBase
	{

		public override Task<ResolutionInfo> Resolve()
		{
			int? nValue = n?.From(ResolutionContext, default);
			int? eValue = e?.From(ResolutionContext, default);
			int? sValue = s?.From(ResolutionContext, default);
			int? wValue = w?.From(ResolutionContext, default);
			int? cValue = c?.From(ResolutionContext, default);
			int? aValue = a?.From(ResolutionContext, default);

			int? turnsOnBoardChange	 = turnsOnBoard?.From(ResolutionContext, default);
			int? attacksThisTurnChange  = attacksThisTurn?.From(ResolutionContext, default);
			int? spacesMovedChange	  = spacesMoved?.From(ResolutionContext, default);
			int? durationChange		 = duration?.From(ResolutionContext, default);

			foreach (var card in cards.From(ResolutionContext, default).Select(c => c.Card))
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
}