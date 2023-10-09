using System.Threading.Tasks;
using Kompas.Effects.Models.Identities.Numbers;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class UpdateCardStats : ChangeCardStatsBase
	{
		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			n ??= new Constant() { constant = 0 };
			e ??= new Constant() { constant = 0 };
			s ??= new Constant() { constant = 0 };
			w ??= new Constant() { constant = 0 };
			c ??= new Constant() { constant = 0 };
			a ??= new Constant() { constant = 0 };
			base.Initialize(eff, subeffIndex);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			int nChange = n.From(ResolutionContext, default);
			int eChange = e.From(ResolutionContext, default);
			int sChange = s.From(ResolutionContext, default);
			int wChange = w.From(ResolutionContext, default);
			int cChange = c.From(ResolutionContext, default);
			int aChange = a.From(ResolutionContext, default);

			int? turnsOnBoardChange	 = turnsOnBoard?.From(ResolutionContext, default);
			int? attacksThisTurnChange  = attacksThisTurn?.From(ResolutionContext, default);
			int? spacesMovedChange	  = spacesMoved?.From(ResolutionContext, default);
			int? durationChange		 = duration?.From(ResolutionContext, default);

			foreach (var card in CardsToAffect)
			{
				ValidateCardOnBoard(card);

				card.AddToStats((nChange, eChange, sChange, wChange, cChange, aChange), Effect);

				if (turnsOnBoardChange.HasValue) card.TurnsOnBoard += turnsOnBoardChange.Value;
				if (attacksThisTurnChange.HasValue) card.AttacksThisTurn += attacksThisTurnChange.Value;
				if (spacesMovedChange.HasValue) card.SpacesMoved += spacesMovedChange.Value;
				if (durationChange.HasValue) card.Duration += durationChange.Value;
			}

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}