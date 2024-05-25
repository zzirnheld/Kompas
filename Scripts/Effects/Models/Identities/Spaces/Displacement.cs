using System;
using Kompas.Cards.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Spaces
{
	public class ApplyDisplacement : ContextualParentIdentityBase<Space>, IIdentity<IGameCardInfo>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> from;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> displacement;
		#nullable restore

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			from.Initialize(initializationContext);
			displacement.Initialize(initializationContext);
		}

		IGameCardInfo? IIdentity<IGameCardInfo>.From(IResolutionContext context, IResolutionContext secondaryContext)
		{
			Space? space = From(context, secondaryContext);
			return InitializationContext.game.Board.GetCardAt(space);
		}

		protected override Space? AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var origin = from.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			var displ = displacement.From(context, secondaryContext)
				?? throw new InvalidOperationException();
			return origin + displ;
		}
	}

	public class Displacement : ContextualParentIdentityBase<Space>
	{
		#nullable disable
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> from;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Space> to;
		#nullable restore

		[JsonProperty]
		public bool subjective = false;

		public override void Initialize(InitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			from.Initialize(initializationContext);
			to.Initialize(initializationContext);
		}

		protected override Space? AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var origin = from.From(context, secondaryContext);
			var destination = to.From(context, secondaryContext);

			if (origin == null || destination == null) return null;

			if (subjective)
			{
				var owner = InitializationContext.Owner
					?? throw new System.NullReferenceException("No owner to draw subjectivity from!");
				origin = owner.SubjectiveCoords(origin);
				destination = owner.SubjectiveCoords(destination);
			}

			return origin.DisplacementTo(destination);
		}
	}
}