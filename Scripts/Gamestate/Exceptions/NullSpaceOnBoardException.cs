using Kompas.Cards.Models;

namespace Kompas.Gamestate.Exceptions
{
	public class NullSpaceOnBoardException
		: KompasException
	{
		public IGameCardInfo Card { get; }

		public NullSpaceOnBoardException(IGameCardInfo card, string? debugMessage = null, string? message = null)
			: base(debugMessage ?? $"{card} was in play, but didn't have a position",
				message ?? $"{card} was in play, but didn't have a position")
		{
			Card = card;
		}
	}
}