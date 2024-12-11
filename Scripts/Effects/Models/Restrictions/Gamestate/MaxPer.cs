using Kompas.Effects.Models.TriggeringEvent;
using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate;

public abstract class MaxPer : GamestateRestrictionBase
{
	[JsonProperty]
	public int max = 1;
	/// <summary>
	/// False indicates should check effect.
	/// </summary>
	[JsonProperty]
	public bool attacks = false;

	protected int Max => max; //Futureproofing in case I want to allow an identity instead, for "x times per turn"

	protected abstract int Uses { get; }

	protected override bool IsValidLogic(IResolutionContext context) => Uses < Max; // ? true : LogFalse();

	//private bool LogFalse() { Logger.Log($"{Uses} exceeded {max} in {InitializationContext.effect}"); return false; }

	public override bool IsStillValidTriggeringContext(IEventContext context)
		=> IsValid(IResolutionContext.NotResolving(context));
}

public class MaxPerTurn : MaxPer
{
	public override int? MaxUsesPerTurn => max;

	protected override int Uses => attacks
		? InitializationContext.source?.AttacksThisTurn
			?? throw new IllDefinedException()
		: InitializationContext.effect?.TimesUsedThisTurn
			?? throw new IllDefinedException();
}

public class MaxPerRound : MaxPer
{
	public override int? MaxUsesPerRound => max;

	protected override int Uses => attacks
		? throw new IllDefinedException()
		: InitializationContext.effect?.TimesUsedThisRound
			?? throw new IllDefinedException();
}

public class MaxPerStack : MaxPer
{
	public override int? MaxUsesPerStack => max;

	protected override int Uses => attacks
		? throw new IllDefinedException()
		: InitializationContext.effect?.TimesUsedThisStack
			?? throw new IllDefinedException();
}