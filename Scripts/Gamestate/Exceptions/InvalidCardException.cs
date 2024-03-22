using Kompas.Cards.Models;

namespace Kompas.Gamestate.Exceptions
{
	public class InvalidCardException : KompasException
	{
		public readonly IGameCard card;

		public InvalidCardException(IGameCard card, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.card = card;
		}
	}
}