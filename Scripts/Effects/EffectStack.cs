using System.Collections.Generic;
using System.Linq;
using Kompas.Effects.Models;

namespace Kompas.Effects
{
	public class EffectStack<StackableType, ContextType>
		where ContextType : IResolutionContext
	{
		private readonly List<(StackableType stackable, ContextType? context)> stack = new();

		public IEnumerable<StackableType> StackEntries => stack.Select(entry => entry.stackable);

		public bool Empty => stack.Count == 0;
		public int Count => stack.Count;

		public void Push((StackableType, ContextType?) entry)
		{
			stack.Add(entry);
		}

		public (StackableType?, ContextType?) Pop()
		{
			if (stack.Count == 0) return (default, default);

			var last = stack.Last();
			stack.Remove(last);
			return last;
		}

		public StackableType? Cancel(int index)
		{
			if (index >= stack.Count) return default;

			var canceled = stack[index].stackable;
			stack.RemoveAt(index);
			return canceled;
		}
	}
}