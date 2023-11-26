using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public class Operation : ContextualParentIdentityBase<int>
	{
		#nullable disable
		[JsonProperty]
		public IIdentity<int>[] numbers;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<int>> manyNumbers;
		[JsonProperty(Required = Required.Always)]
		public INumberOperation operation;
		#nullable restore

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);

			if (AllNull(numbers, manyNumbers)) throw new System.ArgumentException($"Must provide something to perform an operation on");

			manyNumbers?.Initialize(initializationContext);
			if (numbers != null) foreach(var identity in numbers) identity.Initialize(initializationContext);
		}

		protected override int AbstractItemFrom(IResolutionContext? context, IResolutionContext? secondaryContext)
		{
			var numberValues = new List<int>();

			if (numbers != null) foreach (var number in numbers) numberValues.Add(number.From(context, secondaryContext));
			if (manyNumbers != null) foreach (int number in manyNumbers.From(context, secondaryContext)) numberValues.Add(number);

			return operation.Perform(numberValues.ToArray());
		}
	}
}