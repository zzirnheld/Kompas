using System.Threading.Tasks;
using Kompas.Effects.Models.Identities.Numbers;
using Kompas.Shared.Exceptions;

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
			_ = n ?? throw new NotInitializedException();
			_ = e ?? throw new NotInitializedException();
			_ = s ?? throw new NotInitializedException();
			_ = w ?? throw new NotInitializedException();
			_ = c ?? throw new NotInitializedException();
			_ = a ?? throw new NotInitializedException();
			int nChange = n.From(ResolutionContext, ResolutionContext);
			int eChange = e.From(ResolutionContext, ResolutionContext);
			int sChange = s.From(ResolutionContext, ResolutionContext);
			int wChange = w.From(ResolutionContext, ResolutionContext);
			int cChange = c.From(ResolutionContext, ResolutionContext);
			int aChange = a.From(ResolutionContext, ResolutionContext);

			int? turnsOnBoardChange = turnsOnBoard?.From(ResolutionContext, ResolutionContext);
			int? attacksThisTurnChange = attacksThisTurn?.From(ResolutionContext, ResolutionContext);
			int? spacesMovedChange = spacesMoved?.From(ResolutionContext, ResolutionContext);
			int? durationChange = duration?.From(ResolutionContext, ResolutionContext);

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