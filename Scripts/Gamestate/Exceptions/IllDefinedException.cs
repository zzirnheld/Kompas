namespace Kompas.Gamestate.Exceptions
{
	/// <summary>
	/// Indicates that the initializeable item was initialized, but incorrectly,
	/// so the operation is ill-defined
	/// </summary>
	public class IllDefinedException : KompasException
	{
		public IllDefinedException(string? debugMessage = null, string? message = null)
			: base(debugMessage, message)
		{
		}
	}
}