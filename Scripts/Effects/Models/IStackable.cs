using Kompas.Cards.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models;

/// <summary>
/// Some entity that can be resolved on the stack.
/// This term was invented before I migrated to use <see cref="IStackableResolution"/>,
/// but it's what I've got.
/// </summary>
public interface IStackable
{
	/// <summary>
	/// The card that this stackable is from, if any.
	/// Nullable because, for example, a <see cref="HandSizeStackable"/> doesn't have a source card
	/// </summary>
	public GameCard? Card { get; }

	//TODO move this to IResolutionContext
	public IPlayer? ControllingPlayer { get; }

	/// <returns>
	/// The card that caused some downstream effect of this stackable to occur.
	/// See <seealso cref="TriggeringEvent.IEventContext.CauseCardAfter"/>
	/// </returns>
	/// <remarks>
	/// In almost every case, this is the same as <see cref="Card"/>,
	/// but with one notable exception:
	/// during an attack, anything that happens to the defender or its augments is caused by the attacker;
	/// and any thing that happens to the attacker or its augments is caused by the defender.
	/// </remarks>
	public GameCard? GetCause(IGameCardInfo? withRespectTo);

	/// <summary>
	/// The blurb should be displayed at the start of each of this stackable's resolutions.
	/// See <see cref="IResolutionContext.Blurb"/> 
	/// </summary>
	public string InitialBlurb { get; }
}