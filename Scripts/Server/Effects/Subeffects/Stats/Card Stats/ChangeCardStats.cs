using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Cards;
using Kompas.Effects.Models.Identities.ManyCards;
using Kompas.Effects.Models.Identities.Numbers;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class ChangeCardStatsData : CardStatChangeDataBase
{
	//IIdentities take precendence over the modifier/divisor/multiplier variables from legacy cards
	[JsonProperty]
	public int nModifier = 0;
	[JsonProperty]
	public int eModifier = 0;
	[JsonProperty]
	public int sModifier = 0;
	[JsonProperty]
	public int wModifier = 0;
	[JsonProperty]
	public int cModifier = 0;
	[JsonProperty]
	public int aModifier = 0;

	[JsonProperty]
	public int nDivisor = 1;
	[JsonProperty]
	public int eDivisor = 1;
	[JsonProperty]
	public int sDivisor = 1;
	[JsonProperty]
	public int wDivisor = 1;
	[JsonProperty]
	public int cDivisor = 1;
	[JsonProperty]
	public int aDivisor = 1;

	[JsonProperty]
	public int nMultiplier = 0;
	[JsonProperty]
	public int eMultiplier = 0;
	[JsonProperty]
	public int sMultiplier = 0;
	[JsonProperty]
	public int wMultiplier = 0;
	[JsonProperty]
	public int cMultiplier = 0;
	[JsonProperty]
	public int aMultiplier = 0;
	
}

public class ChangeCardStats : CardStatChangeBase
{
	//Can't refactor to coalesce with SetCardStats because I want to lock in non-nullability here
	protected readonly IIdentity<int> n;
	protected readonly IIdentity<int> e;
	protected readonly IIdentity<int> s;
	protected readonly IIdentity<int> w;
	protected readonly IIdentity<int> c;
	protected readonly IIdentity<int> a;

	public ChangeCardStats(ChangeCardStatsData data) : base(data)
	{
		n = data.n ?? new EffectX() { multiplier = data.nMultiplier, modifier = data.nModifier, divisor = data.nDivisor };
		e = data.e ?? new EffectX() { multiplier = data.eMultiplier, modifier = data.eModifier, divisor = data.eDivisor };
		s = data.s ?? new EffectX() { multiplier = data.sMultiplier, modifier = data.sModifier, divisor = data.sDivisor };
		w = data.w ?? new EffectX() { multiplier = data.wMultiplier, modifier = data.wModifier, divisor = data.wDivisor };
		c = data.c ?? new EffectX() { multiplier = data.cMultiplier, modifier = data.cModifier, divisor = data.cDivisor };
		a = data.a ?? new EffectX() { multiplier = data.aMultiplier, modifier = data.aModifier, divisor = data.aDivisor };
	}

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);

		var initContext = DefaultInitializationContext;
		n.Initialize(initContext);
		e.Initialize(initContext);
		s.Initialize(initContext);
		w.Initialize(initContext);
		c.Initialize(initContext);
		a.Initialize(initContext);
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
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