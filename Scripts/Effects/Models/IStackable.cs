using Kompas.Cards.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public interface IStackable
	{
		Player ControllingPlayer { get; }
		GameCard Source { get; }

		GameCard GetCause (IGameCard withRespectTo);
	}
}