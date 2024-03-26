using System.Collections.Generic;
using System.Linq;
using Godot;
using Kompas.Shared.Enumerable;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Restrictions
{
	public abstract class AllOfBase<RestrictedType> : AllOfBase<RestrictedType, IRestriction<RestrictedType>> { }
	
	public abstract class AllOfBase<RestrictedType, ElementRestrictionType>
		: RestrictionBase<RestrictedType>, IAllOf<RestrictedType>
		where ElementRestrictionType : IRestriction<RestrictedType>	
	{
		[JsonProperty] //Not always required because default elems exist
		public IList<ElementRestrictionType> elements = new List<ElementRestrictionType>();

		protected virtual bool LogSoloElements => true;
		protected virtual IEnumerable<ElementRestrictionType> DefaultElements => Enumerable.Empty<ElementRestrictionType>();

		private IAllOf<RestrictedType>.ShouldIgnore ignorePredicate = elem => false;

		public IEnumerable<IRestriction<RestrictedType>> GetElements()
		{
			foreach (var elem in elements) yield return elem;
		}

		public bool IsValidIgnoring(RestrictedType? item, IResolutionContext context, IAllOf<RestrictedType>.ShouldIgnore ignorePredicate)
		{
			var oldVal = this.ignorePredicate;
			this.ignorePredicate = ignorePredicate;
			bool ret = IsValid(item, context);
			this.ignorePredicate = oldVal;
			return ret;
		}

		public override void Initialize(InitializationContext initializationContext)
		{
			elements = elements.Safe()
				.Concat(DefaultElements)
				.ToList();
			base.Initialize(initializationContext);
			var childInitializationContext = initializationContext.Child(this);
			foreach (var element in elements) element.Initialize(childInitializationContext);

			//Debug log for eliminiating trivial AllOfs
			if (LogSoloElements && elements.Count == 1) Logger.Log($"only one element on {GetType()} on eff of {initializationContext.source}");
		}

		protected override bool IsValidLogic(RestrictedType? item, IResolutionContext context) => elements
			.Where(r => !ignorePredicate(r))
			.All(r => Validate(r, item, context));


		private const bool DEBUG = true; //TODO factor out to global flag/checkbox
		/// <summary>
		/// Override if you want to change the validation function called on each child,
		/// like have a client-side variant
		/// </summary>
		protected virtual bool Validate(ElementRestrictionType element, RestrictedType? item, IResolutionContext context)
		{
			bool ret = element.IsValid(item, context);
			if (DEBUG && !ret) Logger.Log($"{item} failed by {element}");
			return ret;
		}
	}
}