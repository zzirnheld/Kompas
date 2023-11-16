using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions
{
	public abstract class DualRestrictionBase<RestrictedType> : RestrictionBase<RestrictedType>
	{
		protected abstract IEnumerable<IRestriction<RestrictedType>> DefaultRestrictions { get; }

		/// <summary>
		/// A piece of a movement restriction.
		/// Basically, a movement restriction is made up of two groups of restrictions -
		/// one that's checked for a normal move (i.e. player-initiated during an open gamestate),
		/// and one that's checked when the card moves by effect
		/// </summary>
		protected class DualComponentRestriction : AllOfBase<RestrictedType>
		{

			private readonly IReadOnlyList<IRestriction<RestrictedType>> restrictions;

			/// <summary>
			/// Constructs a new piece of an overall MovementRestriction
			/// </summary>
			/// <param name="sharedRestrictions">Restrictions that are shared among all elements of the MovementRestriction.
			/// If this is null, the DefaultMovementRestrictions are used instead.</param>
			/// <param name="specificRestrictions">Restrictions that are specific to this particular ComponentMovementRestriction</param>
			public DualComponentRestriction(IEnumerable<IRestriction<RestrictedType>> sharedRestrictions,
				IEnumerable<IRestriction<RestrictedType>> defaultRestrictions,
				params IEnumerable<IRestriction<RestrictedType>>[] specificRestrictions)
			{
				var interimRestrictions = sharedRestrictions ?? defaultRestrictions;
				foreach(var additionalRestrictions in specificRestrictions)
				{
					interimRestrictions = interimRestrictions.Concat(additionalRestrictions);
				}
				this.restrictions = interimRestrictions.ToArray();
			}

			//In this case, "Default" is checked when Initialize is called. since this gets constructed before it's initialized, we're good.
			protected override IEnumerable<IRestriction<RestrictedType>> DefaultElements => restrictions;
		}

		#nullable disable
		protected DualComponentRestriction NormalRestriction { get; private set; }
		protected DualComponentRestriction EffectRestriction { get; private set; }

		[JsonProperty]
		public IRestriction<RestrictedType>[] normalAndEffect = null;
		#nullable restore
		[JsonProperty]
		public IRestriction<RestrictedType>[] normalOnly = System.Array.Empty<IRestriction<RestrictedType>>();
		[JsonProperty]
		public IRestriction<RestrictedType>[] effectOnly = System.Array.Empty<IRestriction<RestrictedType>>();

		/// <summary>
		/// Restrictions that, by default, apply to a player moving a card normally (but not by effect)
		/// </summary>
		protected abstract IEnumerable<IRestriction<RestrictedType>> DefaultNormalRestrictions { get; }

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			
			NormalRestriction = new DualComponentRestriction(sharedRestrictions: normalAndEffect, defaultRestrictions: DefaultRestrictions,
				DefaultNormalRestrictions, normalOnly);
			NormalRestriction.Initialize(initializationContext);

			EffectRestriction = new DualComponentRestriction(sharedRestrictions: normalAndEffect, defaultRestrictions: DefaultRestrictions,
				effectOnly);
			EffectRestriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(RestrictedType? item, IResolutionContext? context)
			=> IsValidIgnoring(item, context, r => false);

		protected bool IsValidIgnoring(RestrictedType? item, IResolutionContext? context, IAllOf<RestrictedType>.ShouldIgnore ignorePredicate)
			=> context?.TriggerContext.stackableCause == null
				? NormalRestriction.IsValidIgnoring(item, context, ignorePredicate)
				: EffectRestriction.IsValidIgnoring(item, context, ignorePredicate);
	}
}