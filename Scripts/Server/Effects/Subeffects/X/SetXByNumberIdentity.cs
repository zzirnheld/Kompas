using Kompas.Effects.Models.Identities;

namespace Kompas.Server.Effects.Subeffects
{
	public class SetXByNumberIdentity: SetX
	{
		public IIdentity<int> numberIdentity;

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