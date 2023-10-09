using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Spaces
{
	public class ApplyDisplacement : ContextualParentIdentityBase<Space>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> from;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> displacement;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			from.Initialize(initializationContext);
			displacement.Initialize(initializationContext);
		}

		protected override Space AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> from.From(context, secondaryContext) + displacement.From(context, secondaryContext);
	}

	public class Displacement : ContextualParentIdentityBase<Space>
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> from;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> to;

		[JsonProperty]
		public bool subjective = false;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			from.Initialize(initializationContext);
			to.Initialize(initializationContext);
		}

		protected override Space AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var origin = from.From(context, secondaryContext);
			var destination = to.From(context, secondaryContext);

			if (subjective)
			{
				origin = InitializationContext.Owner.SubjectiveCoords(origin);
				destination = InitializationContext.Owner.SubjectiveCoords(destination);
			}

			return origin.DisplacementTo(destination);
		}
	}
}