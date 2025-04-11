using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Kompas.Effects.Models;

public interface IStackableResolution
{
	public IStackable Stackable { get; }
	public IResolutionContext Context { get; }
}

public interface IStackableResolution<out StackableType>
	: IStackableResolution
	where StackableType : IStackable
{
	public new StackableType Stackable { get; }
}

public interface IStackableResolution<out StackableType, out ContextType>
	: IStackableResolution<StackableType>
	where StackableType : IStackable
	where ContextType : IResolutionContext
{
	public new ContextType Context { get; }
}

public class StackableResolutionEqualityComparer
	: IEqualityComparer<IStackableResolution>
{
	public bool Equals(IStackableResolution? x, IStackableResolution? y)
	{
		if (object.ReferenceEquals(x, y)) return true;
		if (x is null || y is null) return x == y;

		return Equals(x.Stackable, y.Stackable)
			&& Equals(x.Context, y.Context);
	}

	public int GetHashCode([DisallowNull] IStackableResolution obj) => obj.GetHashCode();
}