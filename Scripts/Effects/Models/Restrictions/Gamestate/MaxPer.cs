using Kompas.Gamestate.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public abstract class MaxPer : GamestateRestrictionBase
	{
		[JsonProperty]
		public int max = 1;

		protected int Max => max; //Futureproofing in case I want to allow an identity instead, for "x times per turn"

		protected abstract int Uses { get; }

		protected override bool IsValidLogic(IResolutionContext context) => Uses < Max; // ? true : LogFalse();

		//private bool LogFalse() { GD.Print($"{Uses} exceeded {max} in {InitializationContext.effect}"); return false; }
	}

	public class MaxPerTurn : MaxPer
	{
		protected override int Uses => InitializationContext.effect?.TimesUsedThisTurn
			?? throw new IllDefinedException();
	}

	public class MaxPerRound : MaxPer
	{
		protected override int Uses => InitializationContext.effect?.TimesUsedThisRound
			?? throw new IllDefinedException();
	}

	public class MaxPerStack : MaxPer
	{
		protected override int Uses => InitializationContext.effect?.TimesUsedThisStack
			?? throw new IllDefinedException();
	}
}