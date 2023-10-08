using System.Collections.Generic;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	/// <summary>
	/// Effects will only be resolved on server. Clients will just get to know what effects they can use
	/// </summary>
	public abstract class Effect : IStackable
	{
		public abstract IGame Game { get; }

		public int EffectIndex { get; private set; }
		public GameCard Source { get; private set; }

		//subeffects
		public abstract Subeffect[] Subeffects { get; }
		/// <summary>
		/// Current subeffect that's resolving
		/// </summary>
		public int SubeffectIndex { get; protected set; }

		//Targets
		public IList<GameCard> CardTargets => CurrentResolutionContext.CardTargets;
		public IList<Space> SpaceTargets => CurrentResolutionContext.SpaceTargets;
		public IList<GameCardInfo> CardInfoTargets => CurrentResolutionContext.CardInfoTargets;
		public IList<IStackable> StackableTargets => CurrentResolutionContext.StackableTargets;

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
			get => CurrentResolutionContext.X;
			set => CurrentResolutionContext.X = value;
		}

		//Triggering and Activating
		public abstract Trigger Trigger { get; }
		public TriggerData triggerData;
		public IActivationRestriction activationRestriction;

		//Misc effect info
		public string blurb;
		public int arg; //used for keyword arguments, and such

		public abstract IResolutionContext CurrentResolutionContext { get; }
		public TriggeringEventContext CurrTriggerContext => CurrentResolutionContext.TriggerContext;
		public int TimesUsedThisTurn { get; protected set; }
		public int TimesUsedThisRound { get; protected set; }
		public int TimesUsedThisStack { get; set; }

		public virtual bool Negated { get; set; }

		/// <summary>
		/// The keyword this effect is from, if it's a full keyword
		/// </summary>
		public string Keyword { get; set; }

		protected virtual void SetInfo(GameCard source, int effIndex, IPlayer owner)
		{
			GD.Print($"Trying to init eff {effIndex} of {source}, with game {Game}");
			Source = source ?? throw new System.ArgumentNullException(nameof(source), "Effect cannot be attached to null card");
			EffectIndex = effIndex;

			blurb = string.IsNullOrEmpty(blurb) ? $"Effect of {source.CardName}" : blurb;
			activationRestriction?.Initialize(new EffectInitializationContext(game: Game, source: Source, effect: this));
			TimesUsedThisTurn = 0;
		}

		public void ResetForTurn(IPlayer turnPlayer)
		{
			TimesUsedThisTurn = 0;
			if (turnPlayer == Source.ControllingPlayer) TimesUsedThisRound = 0;
		}

		public void Reset()
		{
			TimesUsedThisRound = 0;
			TimesUsedThisTurn = 0;
		}

		public virtual bool CanBeActivatedBy(IPlayer controller)
			=> Trigger == null && activationRestriction != null && activationRestriction.IsValid(controller, ResolutionContext.PlayerTrigger(this, Game, controller));

		public virtual bool CanBeActivatedAtAllBy(IPlayer activator)
			=> Trigger == null && activationRestriction != null && activationRestriction.IsPotentiallyValidActivation(activator);

		public GameCard GetTarget(int num) => EffectHelper.GetItem(CardTargets, num);
		public Space GetSpace(int num) => EffectHelper.GetItem(SpaceTargets, num);
		public IPlayer GetPlayer(int num) => EffectHelper.GetItem(playerTargets, num);


		public virtual void AddTarget(GameCard card) {
			CardTargets.Add(card);
		}
		public virtual void RemoveTarget(GameCard card) => CardTargets.Remove(card);

		public void AddSpace(Space space) => SpaceTargets.Add(space.Copy);

		public T TestWithCardTarget<T>(GameCard target, System.Func<T> toTest)
		{
			CardTargets.Add(target);
			var ret = toTest();
			CardTargets.RemoveAt(CardTargets.Count - 1);
			return ret;
		}


		public override string ToString() => $"Effect of {(Source == null ? "Nothing???" : Source.CardName)}";

		public GameCard GetCause(IGameCard withRespectTo) => Source;

		public EffectInitializationContext CreateInitializationContext(Subeffect subeffect, Trigger trigger)
			=> new(game: Game, source: Source, effect: this, trigger: trigger, subeffect: subeffect);
	}
}