using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Subeffects;
using Newtonsoft.Json;

namespace Kompas.Effects.Models;

public class EffectData
{
	[JsonProperty(Required = Required.Always)]
	public SubeffectData[]? Subeffects;

	[JsonProperty]
	public TriggerData? triggerData;
	[JsonProperty]
	public IActivationRestriction? activationRestriction;
	[JsonProperty]
	public string? initialBlurb;
	[JsonProperty]
	public int arg; //used for keyword arguments, and such
}