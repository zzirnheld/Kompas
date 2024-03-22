using Kompas.Cards.Models;

namespace Kompas.Gamestate.Exceptions
{
	public class AlreadyKnownException : KompasException
	{
		public readonly IGameCard card;

		public AlreadyKnownException(IGameCard card, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.card = card;
		}
	}
}