using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Players
{
	public class AllOf : AllOfBase<IPlayer> { }

	public class Not : PlayerRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IRestriction<IPlayer> negated;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			negated.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(IPlayer? item, IResolutionContext context)
			=> !negated.IsValid(item, context);
	}
}