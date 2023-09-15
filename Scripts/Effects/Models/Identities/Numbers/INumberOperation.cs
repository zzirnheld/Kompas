using System.Linq;

namespace Kompas.Effects.Models.Identities.Numbers
{
	public interface INumberOperation
	{
		public int Perform(params int[] numbers);
	}

	namespace NumberOperations
	{
		public class Sum : INumberOperation
		{
			public int Perform(params int[] numbers)
			{
				return numbers.Sum();
			}
		}

		public class Max : INumberOperation
		{
			public int Perform(params int[] numbers)
			{
				return numbers.Max();
			}
		}
	}
}