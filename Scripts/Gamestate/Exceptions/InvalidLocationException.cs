using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;

namespace Kompas.Gamestate.Exceptions
{
	public class InvalidLocationException : KompasException
	{
		public readonly Location location;
		public readonly IGameCard card;

		public InvalidLocationException(Location location, IGameCard card, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.location = location;
			this.card = card;
		}
	}
}