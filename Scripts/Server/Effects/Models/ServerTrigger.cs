using System;
using System.Linq;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Server.Effects.Models;

public class ServerTrigger : Trigger
{
	public ServerEffect ServerEffect { get; private set; }

	public override GameCard Card => ServerEffect.Card;
	public override Effect Effect => ServerEffect;

	/// <summary>
	/// Represents the order this trigger has been given, amongst other simultaneously triggered triggers.
	/// </summary>
	private int order = -1;
	public int Order
    {
        get => order; set => order = value;
    }
    public bool Ordered => order != -1;

	private ServerTrigger(TriggerData triggerData, ServerEffect serverEffect) : base(triggerData, serverEffect)
	{
		if (!TriggerConditions.Contains(triggerData.triggerCondition))
			throw new System.ArgumentNullException(nameof(triggerData), $"invalid trigger condition for effect of {serverEffect.Card.CardName}");

		ServerEffect = serverEffect;

	}

	public static ServerTrigger Create(TriggerData triggerData, ServerEffect serverEffect)
	{
		var ret = new ServerTrigger(triggerData, serverEffect);
		var condition = triggerData.triggerCondition
			?? throw new NullReferenceException($"No trigger condition for trigger {triggerData}!");
		serverEffect.ServerGame.StackController.RegisterTrigger(condition, ret);
		return ret;
	}

	/// <summary>
	/// Checks all relevant trigger restrictions, and whether the card is negated
	/// </summary>
	/// <param name="cardTriggerer">The card that triggered this, if any.</param>
	/// <param name="stackTrigger">The effect or attack that triggered this, if any.</param>
	/// <param name="x">If the action that triggered this has a value of x, it goes here. Otherwise, null.</param>
	/// <returns>Whether all restrictions of the trigger are fulfilled.</returns>
	public bool ValidForTriggeringContext(IEventContext context)
		=> !Card.Negated && TriggerRestriction.IsValid(context, IResolutionContext.NotResolving(context));

	/// <summary>
	/// Rechecks any trigger restrictions that might have changed between the trigger triggering and being ordered.
	/// </summary>
	/// <param name="context"></param>
	/// <returns></returns>
	public bool StillValidForContext(IEventContext context)
		=> TriggerRestriction.IsStillValidTriggeringContext(context);
}