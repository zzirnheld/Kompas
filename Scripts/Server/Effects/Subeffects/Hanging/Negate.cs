using System.Collections.Generic;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.TriggeringEvent;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging;

public class NegateData : HangingEffectData
{
	[JsonProperty]
	public bool negated = true;
}

public class Negate : HangingEffectSubeffect<NegateData>
{
	private readonly bool negated;

	public Negate(NegateData data) : base(data)
	{
		negated = data.negated;
	}

	protected override IEnumerable<HangingEffect> CreateHangingEffects(IServerResolutionContext context)
	{
		var tempNegation = new NegationEffect(end: End, fallOff: FallOff,
			source: ServerEffect, currentContext: context,
			target: GetCardTarget(context), negated: negated);
		return new List<HangingEffect>() { tempNegation };
	}

	public class NegationEffect : HangingEffect
	{
		private readonly GameCard target;
		private readonly bool negated;

		public NegationEffect(EndCondition end, EndCondition fallOff,
			ServerEffect source, IResolutionContext currentContext,
			GameCard target, bool negated)
			: base(end, fallOff, source, currentContext, removeIfEnd: false)
		{
			this.target = target;
			this.negated = negated;
			target.SetNegated(negated, source);
		}

		protected override void ResolveLogic(IEventContext context) => target.SetNegated(!negated, Effect);
	}
}