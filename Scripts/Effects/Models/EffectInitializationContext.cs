using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	/// <summary>
	/// An object to hold all the parameters required to initialize any restriction/restriction elemnt
	/// </summary>
	public struct EffectInitializationContext
	{
		public readonly Game game;
		public readonly GameCard source;

		public readonly Effect effect;

		public readonly Trigger trigger;
		public readonly Subeffect subeffect;

		private readonly Player controllerOverride;
		public readonly Player Controller => controllerOverride ?? effect?.Controller ?? source?.Controller;

		public readonly IContextInitializeable parent;

		public EffectInitializationContext(Game game, GameCard source, 
			Effect effect = default, Trigger trigger = default, Subeffect subeffect = default, Player controller = default)
			: this (game, source, effect, trigger, subeffect, controller, default)
		{ }

		private EffectInitializationContext(Game game, GameCard source,
			Effect effect, Trigger trigger, Subeffect subeffect, Player controller, IContextInitializeable parent)
		{
			this.game = game;
			this.source = source;

			this.effect = effect;

			this.trigger = trigger;
			this.subeffect = subeffect;

			this.controllerOverride = controller;

			this.parent = parent;
		}

		public EffectInitializationContext Child(IContextInitializeable parent)
			=> new EffectInitializationContext(game, source, effect, trigger, subeffect, controllerOverride, parent);

		public override string ToString()
		{
			string str = $"Game {game}, Source card {source}";

			if (effect != null) str += $", Effect {effect}";
			if (trigger != null) str += $", Trigger {trigger}";
			if (subeffect != null) str += $", Subeffect {subeffect}";

			return str;
		}
	}
}