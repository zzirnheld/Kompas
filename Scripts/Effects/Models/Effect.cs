using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models;

/// <summary>
/// Effects will only be resolved on server. Clients will just get to know what effects they can use
/// </summary>
public abstract class Effect : IEffect
{
	public abstract IGame Game { get; }

	public int EffectIndex { get; private set; }
	public abstract GameCard Card { get; }
	public abstract IPlayer OwningPlayer { get; }
	public IPlayer ControllingPlayer => OwningPlayer; //FUTURE: effects can change control. for now, assume same player

	//subeffects
	public abstract Subeffect[] Subeffects { get; }
	//Targets
	protected readonly List<CardLink> cardLinks = new();

	//we don't care about informing players of the contents of these. yet. but we might later
	public IdentityOverrides identityOverrides = new();

	//Triggering and Activating
	public abstract Trigger? Trigger { get; }
	public ITriggerRestriction? TriggerRestriction => Trigger?.TriggerRestriction;
	public TriggerData? triggerData;
	public IActivationRestriction? activationRestriction;
	public IActivationRestriction? ActivationRestriction => activationRestriction;

	//Misc effect info
	public string? blurb;
	public string InitialBlurb => blurb ??= $"Effect of {Card.CardName}";
	public int arg; //used for keyword arguments, and such
	private int _timesUsedThisTurn;
	private int _timesUsedThisRound;
	private int _timesUsedThisStack;
	public event System.EventHandler<IEffect>? EffectInformationChanged;

	public int TimesUsedThisTurn
	{
		get => _timesUsedThisTurn;
		protected set
		{
			_timesUsedThisTurn = value;
			EffectInformationChanged?.Invoke(this, this);
		}
	}
	public int TimesUsedThisRound
	{
		get => _timesUsedThisRound;
		protected set
		{
			_timesUsedThisRound = value;
			EffectInformationChanged?.Invoke(this, this);
		}
	}
	public int TimesUsedThisStack
	{
		get => _timesUsedThisStack;
		set
		{
			_timesUsedThisStack = value;
			EffectInformationChanged?.Invoke(this, this);
		}
	}

	public virtual bool Negated { get; set; }

	/// <summary>
	/// The keyword this effect is from, if it's a full keyword
	/// </summary>
	public string? Keyword { get; set; }

	protected void SetInfo(int effIndex)
	{
		EffectIndex = effIndex;

		//TODO go back to a SerializableEffect model. The Subeffects will still be specified "manually" but that's the cross I'll have to bear, I think,
		//unless I want to make a Serializable version of every subeffect. Which might be a good idea anyway (I'd just put them in the same file for convenience)
		if (Card == null) throw new System.NotImplementedException("Card must be already non-null by the time SetInfo is called.");
		activationRestriction?.Initialize(new InitializationContext(game: Game, source: Card, effect: this));
		TimesUsedThisTurn = 0;
	}

	public void ResetForTurn(IPlayer turnPlayer)
	{
		TimesUsedThisTurn = 0;
		//TODO card is null and this is being called
		if (turnPlayer == Card?.ControllingPlayer) TimesUsedThisRound = 0;
	}

	public void Reset()
	{
		TimesUsedThisRound = 0;
		TimesUsedThisTurn = 0;
	}

	public virtual bool CanBeActivatedBy(IPlayer controller)
		=> Trigger == null && activationRestriction != null && activationRestriction.IsValid(controller, IResolutionContext.PlayerAction(controller));

	public virtual bool CanBeActivatedAtAllBy(IPlayer activator)
		=> Trigger == null && activationRestriction != null && activationRestriction.IsPotentiallyValidActivation(activator);

	public T TestWithCardTarget<T>(GameCard? target, System.Func<T> toTest, IResolutionContext context)
	{
		if (target != null) context.CardTargets.Add(target);
		var ret = toTest();
		if (target != null) context.CardTargets.RemoveAt(context.CardTargets.Count - 1);
		return ret;
	}


	public override string ToString() => $"Effect of {(Card == null ? "Nothing???" : Card.CardName)}";

	public GameCard? GetCause(IGameCardInfo? withRespectTo) => Card;

	public InitializationContext CreateInitializationContext(Subeffect subeffect, Trigger? trigger)
		=> new(game: Game, source: Card, effect: this, trigger: trigger, subeffect: subeffect);
}