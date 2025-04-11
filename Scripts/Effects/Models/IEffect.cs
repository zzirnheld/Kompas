using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;

namespace Kompas.Effects.Models;

public interface IEffect : IStackable
{
	public IGame Game { get; }

	public ITriggerRestriction? TriggerRestriction { get; }
	public IActivationRestriction? ActivationRestriction { get; }

	public int TimesUsedThisTurn { get; }
	public int TimesUsedThisRound { get; }
	public int TimesUsedThisStack { get; set; }

	public event System.EventHandler<IEffect>? EffectInformationChanged;

	public bool Negated { get; set; }

	public int EffectIndex { get; }

	public Trigger? Trigger { get; }

	public void Reset();
}

public static class EffectExtensions
{
	public static int? MaxPerTurn(this IEffect effect)
		=> effect.TriggerRestriction?.MaxUsesPerTurn
		?? effect.ActivationRestriction?.MaxUsesPerTurn;

	public static int? MaxPerRound(this IEffect effect)
		=> effect.TriggerRestriction?.MaxUsesPerRound
		?? effect.ActivationRestriction?.MaxUsesPerRound;

	public static int? MaxPerStack(this IEffect effect)
		=> effect.TriggerRestriction?.MaxUsesPerStack
		?? effect.ActivationRestriction?.MaxUsesPerStack;
}