using System.Diagnostics.CodeAnalysis;

namespace Kompas.Effects.Models;

//NOTE: For this to work as a dictionary key, you'll need to provide the given equality comparer.
//Otherwise, you're relying on the Equals function to be implemented like it is here,
//where the generic type isn't used to distinguish between this and the other being compared.
//For example, an IResolvingStackable<ClientEffect> and an IResolvingStackable<IClientStackable>
//have different generic types, so the traditional "obj is thisType other" would say those aren't equal.
//That makes it good practice to use an IEqualityComparer
//when you use things whose implementation might be generic for a dictionary.
//Or, alternately, it makes it bad practice to do this sort of generic-implements-nongeneric-interface
//for something you plan on using as a dictionary key.
//Either way, it's a useful thing to know.
public abstract class StackableResolution<StackableType, ContextType>
	: IStackableResolution<StackableType, ContextType>
	where StackableType : IStackable
	where ContextType : IResolutionContext
{
	IStackable IStackableResolution.Stackable => Stackable;
	public StackableType Stackable { get; }
	IResolutionContext IStackableResolution.Context => Context;
	public ContextType Context { get; }

	public StackableResolution(StackableType stackable, ContextType context)
	{
		Stackable = stackable;
		Context = context;
	}
	public override bool Equals([NotNullWhen(true)] object? obj)
		=> obj is IStackableResolution other && this.Equals(other);

	public bool Equals(IStackableResolution other)
		=> Equals(Stackable, other.Stackable)
		&& Equals(Context, other.Context);

	public override int GetHashCode()
		=> (Stackable, Context).GetHashCode();

	public static bool operator ==(StackableResolution<StackableType, ContextType> left, StackableResolution<StackableType, ContextType> right) => left.Equals(right);
	public static bool operator !=(StackableResolution<StackableType, ContextType> left, StackableResolution<StackableType, ContextType> right) => !(left == right);
}