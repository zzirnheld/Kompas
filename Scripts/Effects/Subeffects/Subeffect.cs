using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Players;
using Kompas.Effects.Subeffects;
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

	public TargetingContext CurrTargetingContext => new()
	{
		cardTargetIndex = Data.targetIndex,
		spaceTargetIndex = Data.spaceIndex,
		cardInfoTargetIndex = Data.cardInfoIndex,
		playerTargetIndex = Data.playerIndex,
		stackableTargetIndex = Data.stackableIndex
	};

	protected abstract Effect? _Effect { get; }
	public Effect Effect => _Effect
		?? throw new NotInitializedException();
	protected abstract IGame? _Game { get; }
	public IGame Game => _Game
		?? throw new NotInitializedException();

	public int SubeffIndex { get; protected set; }

	protected DataType Data { get; }

	protected Subeffect(DataType data)
	{
		Data = data;
	}


	/// <summary>
	/// If the effect uses X, this is the adjusted value of X
	/// </summary>
	public int AdjustX(IResolutionContext context) => (context.X * Data.xMultiplier / Data.xDivisor) + Data.xModifier;

    public GameCard GetCardTarget(IResolutionContext context)
    {
        return context.GetCardTarget(Data.targetIndex)
        	?? throw new NullCardException(TargetWasNull);
    }

    public Space GetSpaceTarget(IResolutionContext context)
    {
        return context.GetSpaceTarget(Data.spaceIndex)
        	?? throw new NullSpaceException(TargetWasNull);
    }

    public IGameCardInfo GetCardInfoTarget(IResolutionContext context)
    {
		return EffectHelper.GetItem(context.CardInfoTargets, Data.cardInfoIndex)
			?? throw new NullCardException(TargetWasNull);
    }

    public IPlayer GetPlayerTarget(IResolutionContext context)
    {
        return context.GetPlayerTarget(Data.playerIndex)
        	?? throw new NullPlayerException(TargetWasNull);
    }

    public IStackable GetStackableTarget(IResolutionContext context)
    {
		return EffectHelper.GetItem(context.StackableTargets, Data.stackableIndex)
			?? throw new NullPlayerException(TargetWasNull);
    }

    public int JumpIndex => EffectHelper.GetItem(Data.jumpIndices
		?? throw new System.InvalidOperationException("No jump indices, but a subeffect needed one!"),
		Data.jumpIndicesIndex);
}