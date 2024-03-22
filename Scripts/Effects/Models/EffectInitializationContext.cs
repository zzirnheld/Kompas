using Kompas.Cards.Models;
using Kompas.Effects.Subeffects;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	/// <summary>
	/// An object to hold all the parameters required to initialize any restriction/restriction elemnt
	/// </summary>
	public readonly struct EffectInitializationContext
	{
		public readonly IGame game;
		public readonly IGameCard? source;

		public readonly Effect? effect;

		public readonly Trigger? trigger;
		public readonly Subeffect? subeffect;

		private readonly IPlayer? ownerOverride;
		public readonly IPlayer? Owner => ownerOverride ?? effect?.OwningPlayer ?? source?.ControllingPlayer;

		public readonly IContextInitializeable? parent;

		public EffectInitializationContext(IGame game, IGameCard? source, 
			Effect? effect = default, Trigger? trigger = default, Subeffect? subeffect = default, IPlayer? controller = default)
			: this (game, source, effect, trigger, subeffect, controller, default)
		{ }

		private EffectInitializationContext(IGame game, IGameCard? source,
			Effect? effect, Trigger? trigger, Subeffect? subeffect, IPlayer? controller, IContextInitializeable? parent)
		{
			this.game = game;
			this.source = source;

			this.effect = effect ?? subeffect?.Effect;

			this.trigger = trigger;
			this.subeffect = subeffect;

			this.ownerOverride = controller;

			this.parent = parent;
		}

		public EffectInitializationContext Child(IContextInitializeable parent)
			=> new(game, source, effect, trigger, subeffect, ownerOverride, parent);

		public override string ToString()
		{
			string str = $"Game {game}, Card card {source}";

			if (effect != null) str += $", Effect {effect}";
			if (trigger != null) str += $", Trigger {trigger}";
			if (subeffect != null) str += $", Subeffect {subeffect}";

			return str;
		}
	}
}