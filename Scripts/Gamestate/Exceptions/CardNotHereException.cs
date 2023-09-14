using Kompas.Cards.Models;
using Kompas.Gamestate.Locations;

namespace Kompas.Gamestate.Exceptions
{
	public class CardNotHereException : KompasException
	{
		public readonly Location location;
		public readonly GameCardBase card;

		public CardNotHereException(Location location, GameCardBase card, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.location = location;
		}
	}
}