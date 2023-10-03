using System.Linq;
using System.Collections.Generic;
using Kompas.Gamestate;
using Kompas.Cards.Models;

namespace Kompas.Effects.Models.Selectors
{
	public interface ISelector<T>
	{
		T Select(IReadOnlyCollection<T> objects);
	}

	public class RandomSelector<T> : ISelector<T>
	{
		readonly System.Random random = new();

		public T Select(IReadOnlyCollection<T> objects) => objects.ElementAt(random.Next(0, objects.Count));
	}

	//Define types that Newtonsoft is capable of loading
	public class Random : RandomSelector<object> {}
	public class RandomSpace : RandomSelector<Space> {}
	public class RandomCard : RandomSelector<IGameCard> {}

	public class SortIndex: ISelector<IGameCard>
	{
		public IGameCard Select(IReadOnlyCollection<IGameCard> objects)
			=> objects.OrderBy(c => c.IndexInList).FirstOrDefault();
	}
}