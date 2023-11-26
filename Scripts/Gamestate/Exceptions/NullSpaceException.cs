using System;

namespace Kompas.Gamestate.Exceptions
{
	public class NullSpaceException : KompasException
	{
		public NullSpaceException(string debugMessage, string message = "")
			: base(debugMessage, message)
		{ }
	}
}