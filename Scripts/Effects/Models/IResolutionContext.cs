using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models;

//TODO: consider adding ControllingPlayer to IResolutionContext
public interface IResolutionContext
{
	/// <summary>
	/// Represents a lack of resolution information.<br/>
	/// Many places, we have the option of passing in a secondary resolution context.<br/>
	/// This can represent many things, usually in the vein of "stashed resolution context",
	/// like for a delayed effect - we want to test against both the triggering event context
	/// + the resolution context when effect resolution was suspended.
	/// Consumers of such restrictions, etc. should only reference the secondary context if they know it's semantically relevant.
	/// </summary>
	public static IResolutionContext Empty
		=> new DummyResolutionContext(null);

	/// <summary>
	/// Wraps a <see cref="IEventContext"/>,
	/// representing triggering conditions existing even while an effect is not currently resolving.
	/// Used to be able to pass into restrictions that could be checked when an event is resolving,
	/// or while a player does something via other gamerules. (i.e. play by effect vs play by normal gamerule)<br/>
	/// <see cref="CanResolve"/> will be false for <see cref="IResolutionContext"/>s created in this way.<br/>
	/// FUTURE: split out the places that take a resolution context into taking a resolution context + a triggering event context,
	/// so it's semantically obvious that the triggering event context can be present even if there is no actual resolution context
	/// </summary>
	/// <param name="IEventContext"></param>
	/// <returns></returns>
	public static IResolutionContext NotResolving(IEventContext? IEventContext)
		=> new DummyResolutionContext(IEventContext);

	/// <summary>
	/// Represents a player taking an action that an Effect might otherwise cause.
	/// Ex: playing a card.
	/// We have to pass in an IResolutionContext to restrictions that might want to check details about how it happened,
	/// so we wrap a IEventContext that simply says a player did it normally.
	/// </summary>
	public static IResolutionContext PlayerAction(IPlayer agent)
		=> NotResolving(new TriggeringEvent.EventContext() { Player = agent });

	/// <summary>
	/// Information describing the event that triggered this effect to occur, if any such event happened. (If it's player-triggered, this is null.) 
	/// </summary>
	public IEventContext? TriggerContext { get; }

	/// <summary>
	/// The index at which this resolution did or should start
	/// </summary>
	public int StartIndex { get; }

	/// <summary>
	/// The cards currently targeted by this resolution of this effect
	/// </summary>
	public IList<GameCard> CardTargets { get; }

	/// <summary>
	/// Like <see cref="CardTargets"/>, but instead of referring to a card at any point in time,
	/// refers to snapshots of cards as they existed at a moment in time.
	/// Ex: Used for copying the stats of a card as they existed at the moment the effect was triggered
	/// </summary>
	public IList<IGameCardInfo> CardInfoTargets { get; }

	public IList<Space> SpaceTargets { get; }
	public IList<IStackable> StackableTargets { get; }
	public IList<IPlayer> PlayerTargets { get; }
	public IList<GameCard> Rest { get; }

	/// <summary>
	/// Whether the player is currently allowed to decline selecting an additional target.
	/// Used both for effects that allow an arbitrary number of targets,
	/// and for optional effects (but since this is Kompas, optional choices occur on resolution)
	/// </summary>
	public bool CanDeclineTarget { get; set; }

	/// <summary>
	/// A single integer referenced in card effects, used when trying to do something in proportion to something else.
	/// We only allow one integer for card design reasons: more than one would make things hard to read.
	/// </summary>
	public int X { get; set; }
	
	/// <summary>
	/// The text that should display to describe what's happening in this (resolution of the) effect.
	/// Ex: "It's in here somewhere!", "Dragonbirth", "Mad-blood-curse"
	/// </summary>
	public string Blurb { get; set; }

	public IResolutionContext Copy { get; }

	public bool CanResolve { get; }


	/// <summary>
	/// Used for places that need a resolution context (like triggers calling any other identity), but to enforce never having 
	/// </summary>
	private class DummyResolutionContext : IResolutionContext
	{
		private const string NotImplementedMessage = "Dummy resolution context should never have resolution information checked. Use the secondary (aka stashed) resolution context instead.";
		public IEventContext? TriggerContext { get; }

		public int StartIndex => throw new System.NotImplementedException(NotImplementedMessage);
		public IList<GameCard> CardTargets => throw new System.NotImplementedException(NotImplementedMessage);
		public IList<IGameCardInfo> CardInfoTargets => throw new System.NotImplementedException(NotImplementedMessage);
		public IList<Space> SpaceTargets => throw new System.NotImplementedException(NotImplementedMessage);
		public IList<IStackable> StackableTargets => throw new System.NotImplementedException(NotImplementedMessage);
		public IList<IPlayer> PlayerTargets => throw new System.NotImplementedException(NotImplementedMessage);
		public IList<GameCard> Rest => throw new System.NotImplementedException(NotImplementedMessage);

		public int X
		{
			get => throw new System.NotImplementedException(NotImplementedMessage);
			set => throw new System.NotImplementedException(NotImplementedMessage);
		}

		public string Blurb
		{
			get => throw new System.NotImplementedException(NotImplementedMessage);
			set => throw new System.NotImplementedException(NotImplementedMessage);
		}

		public IResolutionContext Copy => new DummyResolutionContext(TriggerContext);

		public bool CanResolve => false;

		public bool CanDeclineTarget
		{
			get => throw new System.NotImplementedException(NotImplementedMessage);
			set => throw new System.NotImplementedException(NotImplementedMessage);
		}

		public DummyResolutionContext(IEventContext? triggerContext)
		{
			TriggerContext = triggerContext;
		}
	}
}

public static class ResolutionContextExtensions
{
	public static GameCard? GetCardTarget(this IResolutionContext context, TargetingContext targetingContext)
	{
		var index = targetingContext?.cardTargetIndex;

		return index is null
			? null
			: GetCardTarget(context, index.Value);
	}

	public static GameCard? GetCardTarget(this IResolutionContext context, int index)
		=> EffectHelper.GetItem(context.CardTargets, index);

	public static Space? GetSpaceTarget(this IResolutionContext context, int index)
		=> EffectHelper.GetItem(context.SpaceTargets, index);

	public static IPlayer? GetPlayerTarget(this IResolutionContext context, TargetingContext targetingContext)
	{
		var index = targetingContext?.cardTargetIndex;

		return index is null
			? null
			: GetPlayerTarget(context, index.Value);
	}

	public static IPlayer? GetPlayerTarget(this IResolutionContext context, int index)
		=> EffectHelper.GetItem(context.PlayerTargets, index);

	public static void AddRest(this IResolutionContext context, IEnumerable<GameCard> cards)
	{
		foreach (var card in cards) context.Rest.Add(card);
	}
}