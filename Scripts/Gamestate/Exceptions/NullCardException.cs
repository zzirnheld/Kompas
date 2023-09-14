using System;

namespace Kompas.Gamestate.Exceptions
{
	public class NullCardException : KompasException
	{
		public NullCardException(string debugMessage, string message = "")
			: base(debugMessage, message)
		{ }
	}
}