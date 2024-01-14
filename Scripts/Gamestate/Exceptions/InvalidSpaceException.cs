namespace Kompas.Gamestate.Exceptions
{
	public class InvalidSpaceException : KompasException
	{
		public readonly Space? space;

		public InvalidSpaceException(Space? space, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.space = space;
		}
	}
}