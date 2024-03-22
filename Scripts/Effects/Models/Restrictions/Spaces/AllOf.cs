using Kompas.Effects.Models.Identities;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions.Spaces
{
	public class AllOf : AllOfBase<Space> { }

	public class Not : SpaceRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IRestriction<Space> negated;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			negated.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space? space, IResolutionContext context)
			=> !negated.IsValid(space, context);
	}

	public class AnyOf : AnyOfBase<Space> { }

	public class Empty : SpaceRestrictionBase
	{
		protected override bool IsValidLogic(Space? space, IResolutionContext context)
			=> InitializationContext.game.Board.IsEmpty(space);
	}

	public class Different : SpaceRestrictionBase
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> from;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			from.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space? item, IResolutionContext context)
			=> from.From(context) != item;
	}

	/// <summary>
	/// Spell rule: you can't place a spell where it would block a path between the avatars through spaces that don't contain a spell.
	/// So to be valid, a card has to either not be a spell, or it has to be a valid place to put a spell.
	/// </summary>
	public class SpellRule : SpaceRestrictionBase
	{
		protected override bool IsValidLogic(Space? item, IResolutionContext context)
			=> item != null
			&& (InitializationContext.source?.Type != 'S'
			|| InitializationContext.game.Board.ValidSpellSpaceFor(InitializationContext.source, item));
	}
}