using Kompas.Cards.Models;

namespace Kompas.Gamestate.Exceptions
{
	public class AvatarRetreatedException : KompasException
	{
		public readonly GameCard card;

		public AvatarRetreatedException(GameCard card, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.card = card;
		}
	}
}