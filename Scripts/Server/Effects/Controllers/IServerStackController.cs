using System.Threading.Tasks;
using Kompas.Server.Effects.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Effects.Models.TriggeringEvent;
using System.Collections.Generic;

namespace Kompas.Server.Effects.Controllers;

public interface IServerStackController : IStackController
{
	public void PushToStack(IServerStackableResolution<IServerStackable> stackEntry);

	public Task ResolveNextStackEntry();
	public void Cancel(Effect eff);
	public Task CheckForResponse();

	public void TriggerFor(IEventContext context);

	public void RegisterTrigger(string condition, ServerTrigger trigger);
	public void RegisterHangingEffect(string condition, HangingEffect hangingEff, string? fallOffCondition = default);
}

public static class IServerStackControllerExtensions
{
	public static void TriggerFor(this IServerStackController stack, params IEventContext[] contexts)
		=> TriggerFor(stack, contexts);

	public static void TriggerFor(this IServerStackController stack, IReadOnlyCollection<IEventContext> contexts)
	{
		foreach (var context in contexts) stack.TriggerFor(context);
	}
}
