using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Cards;
using Kompas.Effects.Models.Identities.ManyCards;
using Kompas.Gamestate.Exceptions;
using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;
using System;
using Newtonsoft.Json;
using Kompas.Effects.Subeffects;

namespace Kompas.Server.Effects.Models.Subeffects;

public abstract class ChangeCardStatsDataBase : SubeffectData
{
	[JsonProperty]
	public IIdentity<IGameCardInfo>? card;
	[JsonProperty]
	public IIdentity<IReadOnlyCollection<IGameCardInfo>>? cards;

	[JsonProperty]
	public IIdentity<int>? n;
	[JsonProperty]
	public IIdentity<int>? e;
	[JsonProperty]
	public IIdentity<int>? s;
	[JsonProperty]
	public IIdentity<int>? w;
	[JsonProperty]
	public IIdentity<int>? c;
	[JsonProperty]
	public IIdentity<int>? a;

	[JsonProperty]
	public IIdentity<int>? turnsOnBoard;
	[JsonProperty]
	public IIdentity<int>? attacksThisTurn;
	[JsonProperty]
	public IIdentity<int>? spacesMoved;
	[JsonProperty]
	public IIdentity<int>? duration;
	
}

public abstract class ChangeCardStatsBase : ServerSubeffect
{
	protected readonly IIdentity<IReadOnlyCollection<IGameCardInfo>> cards;

	protected readonly IIdentity<int>? n;
	protected readonly IIdentity<int>? e;
	protected readonly IIdentity<int>? s;
	protected readonly IIdentity<int>? w;
	protected readonly IIdentity<int>? c;
	protected readonly IIdentity<int>? a;

	protected readonly IIdentity<int>? turnsOnBoard;
	protected readonly IIdentity<int>? attacksThisTurn;
	protected readonly IIdentity<int>? spacesMoved;
	protected readonly IIdentity<int>? duration;

	protected ChangeCardStatsBase(ChangeCardStatsDataBase data) : base(data)
	{
		var card = data.card ?? new TargetIndex() { index = data.targetIndex };
		cards = data.cards ?? new Concat() { cards = new IIdentity<IGameCardInfo>[] { card } };

		n = data.n;
		e = data.e;
		s = data.s;
		w = data.w;
		c = data.c;
		a = data.a;

		turnsOnBoard = data.turnsOnBoard;
		attacksThisTurn = data.attacksThisTurn;
		spacesMoved = data.spacesMoved;
		duration = data.duration;
	}

	protected IEnumerable<GameCard> GetCardsToAffect(IServerResolutionContext context)
		=> cards.From(context)
			?.Select(c => c.Card)
			?? throw new InvalidOperationException();

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);


		var initContext = DefaultInitializationContext;
		cards.Initialize(initContext);

		n?.Initialize(initContext);
		e?.Initialize(initContext);
		s?.Initialize(initContext);
		w?.Initialize(initContext);
		c?.Initialize(initContext);
		a?.Initialize(initContext);

		turnsOnBoard?.Initialize(initContext);
		attacksThisTurn?.Initialize(initContext);
		spacesMoved?.Initialize(initContext);
		duration?.Initialize(initContext);
	}

	protected void ValidateCardOnBoard(GameCard card)
	{
		if (forbidNotBoard && card.Location != Location.Board)
			throw new InvalidLocationException(card.Location, card, ChangedStatsOfCardOffBoard);
	}
}