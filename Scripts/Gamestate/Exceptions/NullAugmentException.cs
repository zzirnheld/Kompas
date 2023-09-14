using Kompas.Cards.Models;
using Kompas.Effects.Models;

namespace Kompas.Gamestate.Exceptions
{
	public class NullAugmentException : KompasException
	{
		public readonly IStackable augmentSrc;
		public readonly GameCard augmentedCard;

		public NullAugmentException(IStackable augmentSrc, GameCard augmentedCard, string debugMessage, string message = "")
			: base(debugMessage, message)
		{
			this.augmentSrc = augmentSrc;
			this.augmentedCard = augmentedCard;
		}
	}
}