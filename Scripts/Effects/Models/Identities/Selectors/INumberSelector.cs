using System.Collections.Generic;
using System.Linq;

namespace Kompas.Effects.Models.Selectors.Numbers
{
	public interface INumberSelector
	{
		public int Apply(IReadOnlyCollection<int> numbers);
	}

	namespace NumberSelectors
	{
		public class Maximum : INumberSelector
		{
			public int Apply(IReadOnlyCollection<int> numbers) => numbers.Max();
		}

		public class Minimum : INumberSelector
		{
			public int Apply(IReadOnlyCollection<int> numbers) => numbers.Min();
		}
	}
}