using System.Collections.Generic;
using System.Linq;
using Kompas.Effects.Models;

namespace Kompas.Effects;

/// <summary>
/// Handles logic specifically around the stack structure itself,
/// without any other responsibilities for game logic.
/// Must have the generics as they are so that we can have the <see cref="StackEntries"/> property
/// </summary>
/// <typeparam name="ResolutionType">Type of <see cref="IStackableResolution"/> that this stack carries</typeparam>
/// <typeparam name="StackableType">Type of <see cref="IStackable"/> this stack manages </typeparam>
public class EffectStack<ResolutionType, StackableType>
	where ResolutionType : class, IStackableResolution<StackableType>
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