using Kompas.Cards.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models
{
	public interface IStackable
	{
		GameCard Source { get; }

		GameCard GetCause (IGameCard withRespectTo);
	}
}