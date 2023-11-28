using System.Collections.Generic;
using Kompas.Effects.Models;
using System.Threading.Tasks;
using System.Linq;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models.Restrictions.Gamestate;
using Kompas.Effects.Models.Restrictions.Triggering;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects.Hanging
{
    public abstract class HangingEffectSubeffect : ServerSubeffect
	{
		//BEWARE: once per turn might not work for these as impl rn, because it's kind of ill-defined.
		//this is only a problem if I one day start creating hanging effects that can later trigger once each turn.
		[JsonProperty]
		public IRestriction<TriggeringEventContext> triggerRestriction = new AlwaysValid();
		#nullable disable
		[JsonProperty (Required = Required.Always)]
		public string endCondition;
		#nullable restore

		[JsonProperty]
		public string fallOffCondition = Trigger.Remove;
		[JsonProperty]
		public IRestriction<TriggeringEventContext>? fallOffRestriction;
		
		public virtual bool ContinueResolution => true;

		protected HangingEffect.EndCondition End => new()
		{
			Restriction = triggerRestriction,
			Condition = endCondition
		};

		protected HangingEffect.EndCondition FallOff => new()
		{
			Condition = fallOffCondition,
			Restriction = GetFallOffRestriction()
		};

		private IRestriction<TriggeringEventContext> GetFallOffRestriction()
		{
			var restriction = fallOffRestriction
				?? (fallOffCondition == Trigger.Remove ?
					TriggerRestrictionBase.AllOf(TriggerRestrictionBase.DefaultFallOffRestrictions) :
					new AlwaysValid());
			restriction.Initialize(DefaultInitializationContext);
			return restriction;
		}

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			triggerRestriction.Initialize(DefaultInitializationContext);

			if (triggerRestriction is IAllOf<TriggerRestrictionBase> allOf
				&& TriggerRestrictionBase.ReevalationRestrictions
					.Intersect(allOf.GetElements().Select(elem => elem.GetType()))
					.Any())
			{
				//TODO: test this. it might be that since they're pushed back to the stack it works fine,
				//but then I need to make sure there's no collision between resume 1/turn and initial trigger 1/turn.
				throw new System.ArgumentException("Hanging effect conditions might not currently support once per turns," +
					"or any other restriction type that would need to be reevaluated after being pushed to stack!");
			}
		}

		public override Task<ResolutionInfo> Resolve()
		{
			//create the hanging effects, of which there can be multiple
			var effectsApplied = CreateHangingEffects();

			//each of the effects needs to be registered, and registered for how it could fall off
			foreach (var eff in effectsApplied)
			{
				ServerGame.StackController.RegisterHangingEffect(endCondition, eff, fallOffCondition);
			}

			//after all that's done, make it do the next subeffect
			if (ContinueResolution) return Task.FromResult(ResolutionInfo.Next);
			else return Task.FromResult(ResolutionInfo.End(EndOnPurpose));
		}

		protected abstract IEnumerable<HangingEffect> CreateHangingEffects();
	}
}