namespace Kompas.Shared
{
	public readonly struct Result<T>
	{
		public T Item { get; private init; }
		public bool HasResult { get; private init; }

		public static Result<T> Of(T item) => new()
		{
			Item = item,
			HasResult = true
		};

		public static readonly Result<T> None = new() { HasResult = false };
	}
}