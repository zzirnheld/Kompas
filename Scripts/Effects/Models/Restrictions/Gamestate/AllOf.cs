using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Gamestate.Locations;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Gamestate
{
	public class AllOf : GamestateRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IList<IGamestateRestriction> elements = System.Array.Empty<IGamestateRestriction>();

		protected virtual bool LogSoloElements => true;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var element in elements) element.Initialize(initializationContext);
			if (LogSoloElements && elements.Count == 1) Logger.Err($"only one element on {GetType()} on eff of {initializationContext.source}");
		}

		protected override bool IsValidLogic(IResolutionContext context)
			=> elements.All(r => r.IsValid(context));

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context)
			=> elements.All(elem => elem.IsStillValidTriggeringContext(context));
	}

	public class Not : GamestateRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IGamestateRestriction negated;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			negated.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IResolutionContext context)
			=> !negated.IsValid(context);

		//NOTE: We can't just use IsStillValidTriggeringContext because that function assumes that the restriction previously evaluated to TRUE,
		//and the whole point of Not is that we already know that inverted evaluated to false
		public override bool IsStillValidTriggeringContext(TriggeringEventContext context)
			=> IsValid(context, IResolutionContext.NotResolving(context));
	}
	
	public class AlwaysValid : GamestateRestrictionBase 
	{
		protected override bool IsValidLogic(IResolutionContext context) => true;

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context)
			=> true;
	}

	public class NeverValid : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext context) => false;

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context)
			=> true;
	}

	public class ThisCardInPlay : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext secondaryContext)
		{
			var source = InitializationContext.source ?? throw new System.NullReferenceException("No source card");
			return source.Location == Location.Board;
		}

		public override bool IsStillValidTriggeringContext(TriggeringEventContext context)
			=> true;
	}
}