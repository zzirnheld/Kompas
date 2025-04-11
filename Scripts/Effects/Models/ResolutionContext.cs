using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models;

public class ResolutionContext : IResolutionContext
{
	public IEventContext? TriggerContext { get; }

	// Used for resuming delayed effects
	public int StartIndex { get; }
	public IList<GameCard> CardTargets { get; }
	public IList<IGameCardInfo> CardInfoTargets { get; }
	public IList<Space> SpaceTargets { get; }
	public IList<IStackable> StackableTargets { get; }
	public IList<IPlayer> PlayerTargets { get; } = new List<IPlayer>();
	public IList<GameCard> Rest { get; } = new List<GameCard>();

    public bool CanDeclineTarget { get; set; }

	public int X { get; set; }
	
	public string Blurb { get; set; }

	public bool CanResolve => true;

	/// <summary>
	/// Describes the resolution of an effect that was triggered by the player.<br/>
	/// (NOT a situation in which a player is attempting to do something "normally" - that's what <see cref="IResolutionContext.PlayerAction"/> is for)
	/// </summary>
	public static ResolutionContext PlayerTriggeredEffect(Effect? effect)
		=> new(new EventContext() { StackableEvent = effect }, effect?.InitialBlurb ?? "");

	public ResolutionContext(IEventContext? triggerContext, string blurb)
	: this(triggerContext, 0,
		Enumerable.Empty<GameCard>(),
		Enumerable.Empty<GameCardInfo>(),
		Enumerable.Empty<Space>(),
		Enumerable.Empty<IStackable>(),
		blurb)
	{ }

	public ResolutionContext(IEventContext? triggerContext,
		int startIndex,
		IEnumerable<GameCard> cardTargets,
		IEnumerable<IGameCardInfo> cardInfoTargets,
		IEnumerable<Space> spaceTargets,
		IEnumerable<IStackable> stackableTargets,
		string blurb)
	{
		TriggerContext = triggerContext;
		StartIndex = startIndex;

		CardTargets = Clone(cardTargets);
		CardInfoTargets = Clone(cardInfoTargets);
		SpaceTargets = Clone(spaceTargets);
		StackableTargets = Clone(stackableTargets);

		X = TriggerContext?.X ?? 0;
		Blurb = blurb;
	}

	private static List<T> Clone<T>(IEnumerable<T>? list)
	{
		if (list == null) return new List<T>();
		else return new List<T>(list);
	}

	public IResolutionContext Copy => new ResolutionContext(TriggerContext, StartIndex,
		CardTargets, CardInfoTargets,
		SpaceTargets, StackableTargets,
		Blurb);

    public override string ToString()
	{
		var sb = new System.Text.StringBuilder();
		sb.Append(base.ToString());
		sb.Append(TriggerContext?.ToString());

		if (CardTargets != null) sb.Append($"Targets: {string.Join(", ", CardTargets)}, ");
		if (StartIndex != 0) sb.Append($"Starting at {StartIndex}");

		return sb.ToString();
	}
}