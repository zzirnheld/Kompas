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
	public abstract ISubeffect[] Subeffects { get; }
	//Targets
	protected readonly List<CardLink> cardLinks = new();

	//we don't care about informing players of the contents of these. yet. but we might later
	public IdentityOverrides identityOverrides = new();

	//Triggering and Activating
	public abstract Trigger? Trigger { get; }
	public ITriggerRestriction? TriggerRestriction => Trigger?.TriggerRestriction;
	public IActivationRestriction? ActivationRestriction { get; }

	//Misc effect info
	public string InitialBlurb { get; private set; } = string.Empty;
	private int _timesUsedThisTurn;
	private int _timesUsedThisRound;
	private int _timesUsedThisStack;
	public event System.EventHandler<IEffect>? EffectInformationChanged;
	public int Arg { get; }

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

	public Effect(EffectData data)
	{
		ActivationRestriction = data.activationRestriction;
		InitialBlurb = data.initialBlurb ?? string.Empty;
		Arg = data.arg;
	}

	protected void SetInfo(int effIndex)
	{
		EffectIndex = effIndex;

		if (Card == null) throw new System.NotImplementedException("Card must be already non-null by the time SetInfo is called.");
		ActivationRestriction?.Initialize(new InitializationContext(game: Game, source: Card, effect: this));
		TimesUsedThisTurn = 0;
		if (InitialBlurb == string.Empty) InitialBlurb = $"Effect of {Card.CardName}";
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
		=> Trigger == null && ActivationRestriction != null && ActivationRestriction.IsValid(controller, IResolutionContext.PlayerAction(controller));

	public virtual bool CanBeActivatedAtAllBy(IPlayer activator)
		=> Trigger == null && ActivationRestriction != null && ActivationRestriction.IsPotentiallyValidActivation(activator);

	public T TestWithCardTarget<T>(GameCard? target, System.Func<T> toTest, IResolutionContext context)
	{
		if (target != null) context.CardTargets.Add(target);
		var ret = toTest();
		if (target != null) context.CardTargets.RemoveAt(context.CardTargets.Count - 1);
		return ret;
	}

	public override string ToString() => InitialBlurb;

	public GameCard? GetCause(IGameCardInfo? withRespectTo) => Card;

	public InitializationContext CreateInitializationContext(ISubeffect subeffect, Trigger? trigger)
		=> new(game: Game, source: Card, effect: this, trigger: trigger, subeffect: subeffect);
}