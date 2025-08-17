
using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Cards;
using Kompas.Effects.Models.Identities.ManyCards;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;

namespace Kompas.Server.Effects.Models.Subeffects;

public abstract class CardStatChangeBase : ServerSubeffect
{
	private readonly IIdentity<IReadOnlyCollection<IGameCardInfo>> cards;

	protected readonly IIdentity<int>? turnsOnBoard;
	protected readonly IIdentity<int>? attacksThisTurn;
	protected readonly IIdentity<int>? spacesMoved;
	protected readonly IIdentity<int>? duration;

	protected CardStatChangeBase(CardStatChangeDataBase data) : base(data)
	{
		var card = data.card ?? new TargetIndex() { index = data.targetIndex };
		cards = data.cards ?? new Concat() { cards = new IIdentity<IGameCardInfo>[] { card } };

		turnsOnBoard = data.turnsOnBoard;
		attacksThisTurn = data.attacksThisTurn;
		spacesMoved = data.spacesMoved;
		duration = data.duration;
	}

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);

		var initContext = DefaultInitializationContext;
		
		cards.Initialize(initContext);

		turnsOnBoard?.Initialize(initContext);
		attacksThisTurn?.Initialize(initContext);
		spacesMoved?.Initialize(initContext);
		duration?.Initialize(initContext);
	}

	protected IEnumerable<GameCard> GetCardsToAffect(IServerResolutionContext context)
		=> cards.From(context)?.Select(c => c.Card)
			?? throw new System.InvalidOperationException();
			
	protected void ValidateCardOnBoard(GameCard card)
	{
		if (forbidNotBoard && card.Location != Location.Board)
			throw new InvalidLocationException(card.Location, card, ChangedStatsOfCardOffBoard);
	}
}