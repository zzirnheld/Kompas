using Kompas.Effects.Models.Identities;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class SetXByNumberIdentity: SetX
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<int> numberIdentity;
		#nullable restore

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			/*var ctxt = DefaultRestrictionContext;
			Godot.GD.Print($"Initializing with {ctxt}");
			numberIdentity.Initialize(initializationContext: ctxt);*/
			numberIdentity.Initialize(initializationContext: DefaultInitializationContext);
		}

		public override int BaseCount => numberIdentity.From(ResolutionContext, ResolutionContext);
	}
}