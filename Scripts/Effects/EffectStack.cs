using System.Collections.Generic;
using System.Linq;
using Kompas.Effects.Models;

namespace Kompas.Effects;

public class EffectStack<ResolutionType, StackableType>
	where ResolutionType : class, IResolvingStackable<StackableType>
	where StackableType : IStackable
{
	private readonly List<ResolutionType> stack = new();
	public IEnumerable<StackableType> StackEntries => stack.Select(entry => entry.Stackable);

	public bool Empty => stack.Count == 0;
	public int Count => stack.Count;

	public void Push(ResolutionType entry)
	{
		stack.Add(entry);
	}

	public ResolutionType? Pop()
	{
		if (stack.Count == 0) return null;

		var last = stack.Last();
		stack.Remove(last);
		return last;
	}

	public ResolutionType? Cancel(int index)
	{
		if (index >= stack.Count) return default;

		var canceled = stack[index];
		stack.RemoveAt(index);
		return canceled;
	}
}