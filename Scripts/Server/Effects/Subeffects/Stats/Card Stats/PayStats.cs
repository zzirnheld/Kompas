using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects;

public class PayStatsData : ChangeCardStatsData
{
	
}

public class PayStats : ChangeCardStats
{
	public PayStats(PayStatsData data) : base(data)
	{
		if (data.attacksThisTurn is not null || data.turnsOnBoard is not null || data.duration is not null || data.spacesMoved is not null)
			throw new IllDefinedException("You didn't think it was a good idea to allow people to pay amounts of those stats");

		if (data.cards is not null)
			throw new IllDefinedException("You haven't decided what to do about multiple cards being paid for");
	}

	public override bool IsImpossible(IResolutionContext context, TargetingContext? overrideContext = null)
	{
		var card = context.GetCardTarget(overrideContext.OrElse(CurrTargetingContext));
		var toPay = GetToPay(context);
		return !CanAfford(card, toPay);
	}

	private CardStats GetToPay(IResolutionContext context) => new(
		n.From(context, context),
		e.From(context, context),
		s.From(context, context),
		w.From(context, context),
		c.From(context, context),
		a.From(context, context)
	);

	private static bool CanAfford(IGameCardInfo? card, CardStats toPay)
	{
		return card is not null
			&& card.N >= toPay.n
			&& card.E >= toPay.e
			&& card.S >= toPay.s
			&& card.W >= toPay.w
			&& card.C >= toPay.c
			&& card.A >= toPay.a;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		var cardTarget = GetCardTarget(resolution.Context);
		if (forbidNotBoard && cardTarget.Location != Location.Board)
			throw new InvalidLocationException(cardTarget.Location, cardTarget, ChangedStatsOfCardOffBoard);

		var toPay = GetToPay(resolution.Context);
		if (!CanAfford(cardTarget, toPay))
			return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));

		cardTarget.AddToStats(-1 * toPay, Effect);
		return Task.FromResult(ResolutionInfo.Next);
	}
}