using Kompas.Cards.Models;

namespace Kompas.Gamestate.Exceptions
{
	public class AvatarRetreatedException : KompasException
	{
		public readonly IGameCard card;

		public AvatarRetreatedException(IGameCard card, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.card = card;
		}
	}
}