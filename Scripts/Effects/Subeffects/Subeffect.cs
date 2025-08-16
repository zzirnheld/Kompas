using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Shared.Exceptions;

namespace Kompas.Effects.Subeffects;

/// <summary>
/// Not abstract because it's instantiated as part of loading subeffects
/// </summary>
public abstract class Subeffect<DataType> : ISubeffect
	where DataType : SubeffectData
{
	#region reasons for impossible
	public const string TargetWasNull = "No target to affect";
	public const string NoValidCardTarget = "No valid card to target";
	public const string NoValidSpaceTarget = "No valid space to target";
	public const string ChangedStatsOfCardOffBoard = "Can't change stats of card not on the board";
	public const string MovedCardOffBoard = "Moved card not on the board";
	public const string EndOnPurpose = "Ended early on purpose";

	public const string CantAffordPips = "Can't afford pips";
	public const string CantAffordStats = "Can't afford stats";

	public const string DeclinedFurtherTargets = "Declined further targets";

	//card movement failure
	public const string CouldntDrawAllX = "Couldn't draw all X cards";
	public const string CouldntMillAllX = "Couldn't mill all X cards";

	//misc
	public const string TooMuchEForHeal = "Target already has at least their printed E";
	#endregion reasons for impossible

	protected abstract Effect? _Effect { get; }
	public Effect Effect => _Effect
		?? throw new NotInitializedException();
	protected abstract IGame? _Game { get; }
	public IGame Game => _Game
		?? throw new NotInitializedException();

	public int SubeffIndex { get; protected set; }

	protected Subeffect(DataType data) { }
}