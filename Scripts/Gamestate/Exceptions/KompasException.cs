using System;

namespace Kompas.Gamestate.Exceptions
{
	public class KompasException : Exception
	{
		public string message;

		public KompasException(string? debugMessage, string? message)
			: base(debugMessage)
		{
			this.message = message ?? string.Empty;
		}
	}
}