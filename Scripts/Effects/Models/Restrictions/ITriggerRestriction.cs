using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Effects.Models.Restrictions;

public interface ITriggerRestriction : IRestriction<IEventContext>
{
	public int? MaxUsesPerTurn { get; }
	public int? MaxUsesPerStack { get; }
	public int? MaxUsesPerRound { get; }

	public bool IsStillValidTriggeringContext(IEventContext context);
}