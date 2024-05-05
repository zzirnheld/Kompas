using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Kompas.Effects.Models
{
	public interface IResolvingStackable
	{
		public IStackable Stackable { get; }
		public IResolutionContext? Context { get; }

		public static IResolvingStackable<StackableType, ContextType> Resolving<StackableType, ContextType>
			(StackableType stackable, ContextType? context)
			where StackableType : IStackable
			where ContextType : IResolutionContext
			=> new ResolvingStackable<StackableType, ContextType>(stackable, context);
	}

	public interface IResolvingStackable<out StackableType>
		: IResolvingStackable
		where StackableType : IStackable
	{
		public new StackableType Stackable { get; }
	}

	public interface IResolvingStackable<out StackableType, out ContextType>
		: IResolvingStackable<StackableType>
		where StackableType : IStackable
		where ContextType : IResolutionContext
	{
		public new ContextType? Context { get; }
	}

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
	public readonly struct ResolvingStackable<StackableType, ContextType>
		: IResolvingStackable<StackableType, ContextType>
		where StackableType : IStackable
		where ContextType : IResolutionContext
	{
		IStackable IResolvingStackable.Stackable => Stackable;
		public StackableType Stackable { get; }
		IResolutionContext? IResolvingStackable.Context => Context;
		public ContextType? Context { get; }

        public ResolvingStackable(StackableType stackable, ContextType? context)
		{
			Stackable = stackable;
			Context = context;
		}
		public override bool Equals([NotNullWhen(true)] object? obj)
			=> obj is IResolvingStackable other && this.Equals(other);

		public bool Equals(IResolvingStackable other)
			=> Equals(Stackable, other.Stackable)
			&& Equals(Context, other.Context);

		public override int GetHashCode()
			=> (Stackable, Context).GetHashCode();

        public static bool operator ==(ResolvingStackable<StackableType, ContextType> left, ResolvingStackable<StackableType, ContextType> right) => left.Equals(right);
        public static bool operator !=(ResolvingStackable<StackableType, ContextType> left, ResolvingStackable<StackableType, ContextType> right) => !(left == right);
    }
	public class ResolvingStackableEqualityComparer
		: IEqualityComparer<IResolvingStackable>
	{
		public bool Equals(IResolvingStackable? x, IResolvingStackable? y)
		{
			if (object.ReferenceEquals(x, y)) return true;
			if (x is null || y is null) return x == y;

			return Equals(x.Stackable, y.Stackable)
				&& Equals(x.Context, y.Context);
		}

		public int GetHashCode([DisallowNull] IResolvingStackable obj) => obj.GetHashCode();
	}
}