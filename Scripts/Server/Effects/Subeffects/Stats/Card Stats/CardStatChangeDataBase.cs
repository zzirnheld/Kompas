using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Subeffects;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class CardStatChangeDataBase : SubeffectData
{
	[JsonProperty]
	public IIdentity<IGameCardInfo>? card;
	[JsonProperty]
	public IIdentity<IReadOnlyCollection<IGameCardInfo>>? cards;

	[JsonProperty]
	public IIdentity<int>? turnsOnBoard;
	[JsonProperty]
	public IIdentity<int>? attacksThisTurn;
	[JsonProperty]
	public IIdentity<int>? spacesMoved;
	[JsonProperty]
	public IIdentity<int>? duration;
	
	// Ideally, use IIdentities, but inheritors may introduce fallbacks for legacy cards
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
}