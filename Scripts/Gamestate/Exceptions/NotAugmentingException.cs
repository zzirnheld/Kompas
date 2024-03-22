using Kompas.Cards.Models;

namespace Kompas.Gamestate.Exceptions
{
	public class NotAugmentingException : KompasException
	{
		public readonly IGameCard card;
		public NotAugmentingException(IGameCard card, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.card = card;
		}
	}
}