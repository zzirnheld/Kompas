using System.Collections.Generic;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public interface IEffect : IStackable
	{
		public int TimesUsedThisTurn { get; }
		public int TimesUsedThisRound { get; }
		public int TimesUsedThisStack { get; set; }

		public bool Negated { get; set; }
		public int X { get; set; }

		public int EffectIndex { get; }

		public Trigger? Trigger { get; }

		public void Reset();

		public void AddTarget(GameCard card);
		public void RemoveTarget(GameCard card);
	}

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
		/// <summary>
		/// Current subeffect that's resolving
		/// </summary>
		public int SubeffectIndex { get; protected set; }

		//Targets
		public IList<GameCard> CardTargets => CurrentResolutionContext?.CardTargets
			?? throw new EffectNotResolvingException(this);
		public IList<Space> SpaceTargets => CurrentResolutionContext?.SpaceTargets
			?? throw new EffectNotResolvingException(this);
		public IList<GameCardInfo> CardInfoTargets => CurrentResolutionContext?.CardInfoTargets
			?? throw new EffectNotResolvingException(this);
		public IList<IStackable> StackableTargets => CurrentResolutionContext?.StackableTargets
			?? throw new EffectNotResolvingException(this);

		protected readonly List<CardLink> cardLinks = new();

		//we don't care about informing players of the contents of these. yet. but we might later
		public readonly List<IPlayer> playerTargets = new();
		public readonly List<GameCard> rest = new();

		public IdentityOverrides identityOverrides = new();

		/// <summary>
		/// X value for card effect text (not coordinates)
		/// </summary>
		public int X  
		{
			get => CurrentResolutionContext?.X 
				?? throw new EffectNotResolvingException(this);
			set
			{
				_ = CurrentResolutionContext ?? throw new EffectNotResolvingException(this);
				CurrentResolutionContext.X = value;
			}
		} 

		//Triggering and Activating
		public abstract Trigger? Trigger { get; }
		public TriggerData? triggerData;
		public IActivationRestriction? activationRestriction;

		//Misc effect info
		public string? blurb;
		public int arg; //used for keyword arguments, and such

		public abstract IResolutionContext? CurrentResolutionContext { get; }
		public TriggeringEventContext? CurrTriggerContext => CurrentResolutionContext?.TriggerContext;
		public int TimesUsedThisTurn { get; protected set; }
		public int TimesUsedThisRound { get; protected set; }
		public int TimesUsedThisStack { get; set; }

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
			blurb = string.IsNullOrEmpty(blurb) ? $"Effect of {Card.CardName}" : blurb;
			activationRestriction?.Initialize(new EffectInitializationContext(game: Game, source: Card, effect: this));
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

		public GameCard? GetTarget(int num) => EffectHelper.GetItem(CardTargets, num);
		public Space? GetSpace(int num) => EffectHelper.GetItem(SpaceTargets, num);
		public IPlayer? GetPlayer(int num) => EffectHelper.GetItem(playerTargets, num);


		public virtual void AddTarget(GameCard card) {
			CardTargets.Add(card);
		}
		public virtual void RemoveTarget(GameCard card) => CardTargets.Remove(card);

		public void AddSpace(Space space) => SpaceTargets.Add(space.Copy);

		public T TestWithCardTarget<T>(GameCard? target, System.Func<T> toTest)
		{
			if (target != null) CardTargets.Add(target);
			var ret = toTest();
			if (target != null) CardTargets.RemoveAt(CardTargets.Count - 1);
			return ret;
		}


		public override string ToString() => $"Effect of {(Card == null ? "Nothing???" : Card.CardName)}";

		public GameCard? GetCause(IGameCardInfo? withRespectTo) => Card;

		public EffectInitializationContext CreateInitializationContext(Subeffect subeffect, Trigger? trigger)
			=> new(game: Game, source: Card, effect: this, trigger: trigger, subeffect: subeffect);
	}
}