using Kompas.Cards.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public interface IStackable
	{
		GameCard? Card { get; }
		IPlayer? ControllingPlayer { get; }

		GameCard? GetCause (IGameCardInfo? withRespectTo);
	}
}