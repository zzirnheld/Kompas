using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;
using Kompas.Effects.Models;

namespace Kompas.Effects.Subeffects;

public interface ISubeffect
{
	public IGame Game { get; }
	public Effect Effect { get; }
	public int SubeffIndex { get; }
}