using System.Collections.Generic;
using System.Linq;
using Kompas.Effects.Models;

namespace Kompas.Effects;

public class EffectStack<StackableType, ContextType>
	where StackableType : class, IStackable
	where ContextType : IResolutionContext
{
	private readonly List<IResolvingStackable<StackableType, ContextType>> stack = new();

	public IEnumerable<StackableType> StackEntries => stack.Select(entry => entry.Stackable);

	public bool Empty => stack.Count == 0;
	public int Count => stack.Count;

	public void Push(IResolvingStackable<StackableType, ContextType> entry)
	{
		stack.Add(entry);
	}

	public IResolvingStackable<StackableType, ContextType>? Pop()
	{
		if (stack.Count == 0) return null;

		var last = stack.Last();
		stack.Remove(last);
		return last;
	}

	public IResolvingStackable<StackableType, ContextType>? Cancel(int index)
	{
		if (index >= stack.Count) return default;

		var canceled = stack[index];
		stack.RemoveAt(index);
		return canceled;
	}
}