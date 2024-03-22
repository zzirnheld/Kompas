using Kompas.Cards.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public interface IStackable
	{
		IGameCard? Card { get; }

		IGameCard? GetCause (IGameCardInfo? withRespectTo);
	}
}