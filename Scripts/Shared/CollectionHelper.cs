using System.Collections.Generic;
using System.Linq;
using Kompas.Shared.Enumerable;

namespace Kompas.Shared;

public class CollectionsHelper
{
	private static readonly System.Random rng = new((int)System.DateTime.Now.Ticks);

	public static IReadOnlyCollection<T> Shuffle<T>(IReadOnlyCollection<T> list) => ShuffleInPlace(list.ToArray()).ToArray();

	public static IList<T> ShuffleInPlace<T>(IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			(list[n], list[k]) = (list[k], list[n]);
		}
		return list;
	}
}