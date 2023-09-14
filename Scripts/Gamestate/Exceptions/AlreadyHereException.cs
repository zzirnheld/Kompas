using Kompas.Gamestate.Locations;

namespace Kompas.Gamestate.Exceptions
{
	public class AlreadyHereException : KompasException
	{
		public readonly Location location;

		public AlreadyHereException(Location location, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.location = location;
		}
	}
}