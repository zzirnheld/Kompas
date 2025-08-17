using Kompas.Effects.Models.Identities;
using Kompas.Shared.Exceptions;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class SetXByNumberIdentityData : SetXData
{
	[JsonProperty(Required = Required.Always)]
	public IIdentity<int>? numberIdentity;
}

public class SetXByNumberIdentity : SetX
{
	public IIdentity<int> numberIdentity;

	public SetXByNumberIdentity(SetXByNumberIdentityData data) : base(data)
	{
		numberIdentity = data.numberIdentity
			?? throw new MissingJSONValueException(nameof(numberIdentity), this);
	}

	public override void Initialize(ServerEffect eff, int subeffIndex)
	{
		base.Initialize(eff, subeffIndex);
		/*var ctxt = DefaultRestrictionContext;
		Godot.Logger.Log($"Initializing with {ctxt}");
		numberIdentity.Initialize(initializationContext: ctxt);*/
		numberIdentity.Initialize(initializationContext: DefaultInitializationContext);
	}

	public override int GetBaseCount(IServerResolutionContext context)
	{
		return numberIdentity.From(context, context);
	}
}