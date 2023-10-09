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
			if (LogSoloElements && elements.Count == 1) GD.PrintErr($"only one element on {GetType()} on eff of {initializationContext.source}");
		}

		protected override bool IsValidLogic(IResolutionContext context)
			=> elements.All(r => r.IsValid(context));
	}

	public class Not : GamestateRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IGamestateRestriction negated;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			negated.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IResolutionContext context)
			=> !negated.IsValid(context);
	}
	
	public class AlwaysValid : GamestateRestrictionBase 
	{
		protected override bool IsValidLogic(IResolutionContext context) => true;
	}

	public class NeverValid : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext context) => false;
	}

	public class ThisCardInPlay : GamestateRestrictionBase
	{
		protected override bool IsValidLogic(IResolutionContext secondaryContext)
			=> InitializationContext.source.Location == Location.Board;
	}
}