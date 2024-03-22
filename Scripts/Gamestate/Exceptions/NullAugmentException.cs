using Kompas.Cards.Models;
using Kompas.Effects.Models;

namespace Kompas.Gamestate.Exceptions
{
	public class NullAugmentException : KompasException
	{
		public readonly IStackable? augmentSrc;
		public readonly IGameCard augmentedCard;

		public NullAugmentException(IStackable? augmentSrc, IGameCard augmentedCard, string debugMessage, string message = "")
			: base(debugMessage, message)
		{
			this.augmentSrc = augmentSrc;
			this.augmentedCard = augmentedCard;
		}
	}
}