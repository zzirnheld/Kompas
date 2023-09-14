using Kompas.Cards.Models;

namespace Kompas.Gamestate.Exceptions
{
	public class NotAugmentingException : KompasException
	{
		public readonly GameCard card;
		public NotAugmentingException(GameCard card, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.card = card;
		}
	}
}