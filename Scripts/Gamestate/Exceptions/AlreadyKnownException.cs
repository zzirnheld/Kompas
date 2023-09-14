using Kompas.Cards.Models;

namespace Kompas.Gamestate.Exceptions
{
	public class AlreadyKnownException : KompasException
	{
		public readonly GameCard card;

		public AlreadyKnownException(GameCard card, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.card = card;
		}
	}
}