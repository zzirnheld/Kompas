using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;

namespace Kompas.Gamestate.Exceptions
{
	public class CardNotHereException : KompasException
	{
		public readonly Location location;
		public readonly IGameCardInfo card;

		public CardNotHereException(Location location, IGameCardInfo card, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.card = card;
			this.location = location;
		}
	}
}