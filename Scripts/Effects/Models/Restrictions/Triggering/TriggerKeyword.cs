using System.Linq;
using Kompas.Cards.Loading;
using Kompas.Effects.Models.TriggeringEvent;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Triggering;

public class TriggerKeyword : TriggerRestrictionBase
{
	//If I wanted to remove nullable disable/enable, the pattern I would want would match the NotInitializedExceptions found in Godot code
	#nullable disable
	[JsonProperty(Required = Required.Always)]
	public string keyword;
	//Initialized after json loaded
	private ITriggerRestriction[] elements;
	#nullable restore

	public override void Initialize(InitializationContext initializationContext)
	{
		base.Initialize(initializationContext);
		elements = CardRepository.InstantiateTriggerKeyword(keyword)
			?? throw new System.InvalidOperationException();
		foreach (var elem in elements) elem.Initialize(initializationContext);
	}

	protected override bool IsValidContext(IEventContext context, IResolutionContext secondaryContext)
		=> elements.All(tre => tre.IsValid(context, secondaryContext));

	public override bool IsStillValidTriggeringContext(IEventContext context)
		=> elements.All(tre => tre.IsStillValidTriggeringContext(context));
}