using System;
using Godot;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions;

namespace Kompas.Effects.Models
{
	public class TriggerData
	{
		public string? triggerCondition;
		public IRestriction<TriggeringEventContext>? triggerRestriction;

		public bool optional = false;
		public string? blurb;
		public bool showX = false;
		public int orderPriority = 0; //positive means it goes on the stack after anything, negative before
	}

	public abstract class Trigger
	{
		public const string TurnStart = "Turn Start";
		public const string StackEnd = "Stack End";

		public const string EffectPushedToStack = "Effect Pushed to Stack";

		//change card stats
		public const string Activate = "Activate";
		public const string Deactivate = "Deactivate";
		public const string Negate = "Negate";
		//X will be equal to the change in the stat
		public const string NChange = "N Change";
		public const string EChange = "E Change";
		public const string SChange = "S Change";
		public const string WChange = "W Change";
		public const string CChange = "C Change";
		public const string AChange = "A Change";
		public const string Healed = "Healed";

		//combat
		public const string Defends = "Defend";
		public const string Attacks = "Attack";
		public const string TakeCombatDamage = "Take Combat Damage";
		public const string DealCombatDamage = "Deal Combat Damage";
		public const string Battles = "Battle Start";
		public const string BattleEnds = "Battle End";

		//card moving
		public const string EachDraw = "Each Card Drawn";
		public const string DrawX = "Draw";
		public const string Arrive = "Arrive";
		public const string Play = "Play";
		public const string Discard = "Discard";
		public const string Rehand = "Rehand";
		public const string Reshuffle = "Reshuffle";
		public const string Topdeck = "Topdeck";
		public const string Bottomdeck = "Bottomdeck";
		public const string ToDeck = "To Deck";
		public const string Move = "Move";
		public const string Annhilate = "Annihilate";
		public const string Remove = "Remove";

		//Primary card is the card that left the aoe, secondary is the card whose aoe it left
		public const string LeaveAOE = "Leave AOE";

		public const string AugmentAttached = "Augment Attached"; //when an augment becomes applied to a card.
		public const string AugmentDetached = "Augment Detached";
		public const string Augmented = "Augmented"; //when a card has an augment applied to it

		public const string Revealed = "Revealed";
		public const string Vanish = "Vanish";

		public const string Anything = "Anything";

		public static readonly string[] TriggerConditions = {
			TurnStart, StackEnd, EffectPushedToStack,
			Activate, Deactivate, Negate,
			NChange, EChange, SChange, WChange, CChange, AChange,
			Defends, Attacks, TakeCombatDamage, DealCombatDamage, Battles, BattleEnds,
			EachDraw, DrawX, Arrive, Play, Discard, Rehand, Reshuffle, Topdeck, Bottomdeck, ToDeck, Move, Annhilate, Remove, LeaveAOE,
			AugmentAttached, AugmentDetached, Augmented,
			Revealed, Vanish,
			Anything
		};

		public TriggerData TriggerData { get; }
		public abstract GameCard Card { get; }
		public abstract Effect Effect { get; }

		public string TriggerCondition => TriggerData.triggerCondition
			?? throw new InvalidOperationException("Trigger data didn't have a trigger condition");
		public IRestriction<TriggeringEventContext> TriggerRestriction => TriggerData.triggerRestriction
			?? throw new InvalidOperationException("Trigger data didn't have a trigger restriction");
		public bool Optional => TriggerData.optional;
		public string Blurb => TriggerData.blurb ?? Effect.blurb ?? string.Empty;

		public Trigger(TriggerData triggerData, Effect effect)
		{
			var initializationContext = new EffectInitializationContext(game: effect.Game, source: effect.Card, effect: effect, trigger: this);
			TriggerData = triggerData;
			_ = triggerData.triggerCondition ?? throw new ArgumentNullException(nameof(triggerData), "Null trigger condition!");
			_ = triggerData.triggerRestriction ?? throw new ArgumentNullException(nameof(triggerData), "Null trigger restriction!");
			try
			{
				triggerData.triggerRestriction.Initialize(initializationContext);
			}
			catch (NullReferenceException)
			{
				GD.PrintErr($"Issue initializing {Blurb} trigger of {effect.Card}");
				throw;
			}
		}
	}
}