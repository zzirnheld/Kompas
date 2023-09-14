using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Players
{
	public class AllOf : AllOfBase<Player> { }

	public class Not : PlayerRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IRestriction<Player> negated;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			negated.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Player item, IResolutionContext context)
			=> !negated.IsValid(item, context);
	}
}