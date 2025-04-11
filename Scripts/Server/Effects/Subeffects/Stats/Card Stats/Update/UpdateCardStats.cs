using System.Threading.Tasks;
using Kompas.Effects.Models.Identities.Numbers;
using Kompas.Shared.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects;

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

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		_ = n ?? throw new NotInitializedException();
		_ = e ?? throw new NotInitializedException();
		_ = s ?? throw new NotInitializedException();
		_ = w ?? throw new NotInitializedException();
		_ = c ?? throw new NotInitializedException();
		_ = a ?? throw new NotInitializedException();
		int nChange = n.From(resolution.Context, resolution.Context);
		int eChange = e.From(resolution.Context, resolution.Context);
		int sChange = s.From(resolution.Context, resolution.Context);
		int wChange = w.From(resolution.Context, resolution.Context);
		int cChange = c.From(resolution.Context, resolution.Context);
		int aChange = a.From(resolution.Context, resolution.Context);

		int? turnsOnBoardChange = turnsOnBoard?.From(resolution.Context, resolution.Context);
		int? attacksThisTurnChange = attacksThisTurn?.From(resolution.Context, resolution.Context);
		int? spacesMovedChange = spacesMoved?.From(resolution.Context, resolution.Context);
		int? durationChange = duration?.From(resolution.Context, resolution.Context);

		foreach (var card in GetCardsToAffect(resolution.Context))
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