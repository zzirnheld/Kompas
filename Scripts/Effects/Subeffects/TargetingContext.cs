namespace Kompas.Effects.Subeffects;

public class TargetingContext
{
	public int? cardTargetIndex;
	public int? spaceTargetIndex;
	public int? cardInfoTargetIndex;
	public int? playerTargetIndex;
	public int? stackableTargetIndex;
}

public static class TargetingContextExtensions
{
	public static TargetingContext OrElse
		(this TargetingContext? context, TargetingContext? other) => new()
	{
		cardTargetIndex = context?.cardTargetIndex ?? other?.cardTargetIndex,
		spaceTargetIndex = context?.spaceTargetIndex ?? other?.spaceTargetIndex,
		cardInfoTargetIndex = context?.cardInfoTargetIndex ?? other?.cardInfoTargetIndex,
		playerTargetIndex = context?.playerTargetIndex ?? other?.playerTargetIndex,
		stackableTargetIndex = context?.stackableTargetIndex ?? other?.stackableTargetIndex,
	};
}