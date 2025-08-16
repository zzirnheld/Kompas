using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Cards.Movement;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging;

public class AnnihilationData : HangingEffectData { }

public class Annihilation : HangingEffectSubeffect
{
	public Annihilation(AnnihilationData data) : base(data) { }

	protected override IEnumerable<HangingEffect> CreateHangingEffects(IServerResolutionContext context)
	{
		var eff = new AnnihilationEffect(end: End, fallOff: FallOff,
			sourceEff: ServerEffect, resolutionContext: context,
			target: GetCardTarget(context));
		return new List<HangingEffect>() { eff };
	}

	/// <summary>
	/// Does nothing when created. When resolves, annihilates its target
	/// </summary>
	private class AnnihilationEffect : HangingEffect
	{
		private readonly GameCard target;

		public AnnihilationEffect(EndCondition end, EndCondition fallOff,
			ServerEffect sourceEff, IResolutionContext resolutionContext, GameCard target)
			: base(end, fallOff, sourceEff, resolutionContext, removeIfEnd: true)
		{
			this.target = target;
		}

		protected override void ResolveLogic(IEventContext context) => target.Annihilate(Effect);

		public override string ToString()
		{
			return $"{base.ToString()} affecting {target}";
		}
	}
}