using System.Collections.Generic;
using System.Linq;

namespace Kompas.Shared.Enumerable
{
	public static class EnumerableHelper
	{
		public static IEnumerable<(int index, T value)> Enumerate<T>(this IEnumerable<T> coll)
				=> coll.Select((i, val) => (val, i));

		public static IEnumerable<T> Safe<T>(this IEnumerable<T> source)
		{
			if (source == null) yield break;

			foreach (var item in source) yield return item;
		}

		public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T elem)
			=> source.Concat(new[] { elem });
	}
}