using System.Collections.Generic;
using System.Linq;

namespace Kompas.Effects.Models.TriggeringEvent;

public class EventCapturer
{
	private readonly IReadOnlyCollection<IIncompleteEventContext> incompletes;

	public EventCapturer(IEnumerable<IIncompleteEventContext> incompletes)
	{
		this.incompletes = incompletes.ToArray();
	}

	public delegate void CapturableEvent();

	public static IReadOnlyCollection<IEventContext> Capture(IEnumerable<IIncompleteEventContext> incompletes, CapturableEvent capturableEvent)
	{
		var capturer = new EventCapturer(incompletes);
		return capturer.Capture(capturableEvent);
	}

	public static IReadOnlyCollection<IEventContext> Capture(CapturableEvent capturableEvent, params IIncompleteEventContext[] incompletes)
		=> Capture(incompletes, capturableEvent);

	public IReadOnlyCollection<IEventContext> Capture(CapturableEvent capturableEvent)
	{
		capturableEvent();
		return incompletes
			.Select(incomplete => incomplete.CacheAfterEvent())
			.ToArray();
	}
}