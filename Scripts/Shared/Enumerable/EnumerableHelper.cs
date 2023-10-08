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

		public static IList<T> AddRangeWithCast<T, U>(this IList<T> list, IEnumerable<U> enumerable)
			where T : U //This doesn't validate that the cast *will* be possible, but at least enforces that it's valid one way
		{
			foreach(var obj in enumerable)
			{
				if (obj is T t) list.Add(t);
				else throw new System.ArgumentException($"Mismatch between type of object {obj.GetType()} and type parameter for adding range {typeof(T)}");
			}
			return list;
		}


		/// <summary>
        /// ElementAt, but allows negative indices to index from the end
        /// </summary>
		public static T ElementAtWrapped<T>(this IEnumerable<T> source, int index)
			=> source.ElementAtOrDefault(TrueIndex(source.Count(), index));

		public static int TrueIndex(int len, int index) => index < 0 ? index + len : index;
	}
}