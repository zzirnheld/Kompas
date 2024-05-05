using System.Collections;
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
			=> obj is ResolvingStackable<StackableType, ContextType> other && this.Equals(other);

		public bool Equals(ResolvingStackable<StackableType, ContextType> other)
			=> Equals(Stackable, other.Stackable)
			&& Equals(Context, other.Context);

		public override int GetHashCode()
			=> (Stackable, Context).GetHashCode();

        public static bool operator ==(ResolvingStackable<StackableType, ContextType> left, ResolvingStackable<StackableType, ContextType> right) => left.Equals(right);
        public static bool operator !=(ResolvingStackable<StackableType, ContextType> left, ResolvingStackable<StackableType, ContextType> right) => !(left == right);
    }
}