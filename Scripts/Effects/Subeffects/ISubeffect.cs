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
	public int JumpIndex { get; }

	/// <summary>
	/// If the effect uses X, this is the adjusted value of X
	/// </summary>
	public int AdjustX(IResolutionContext context);

	public GameCard GetCardTarget(IResolutionContext context);

	public Space GetSpaceTarget(IResolutionContext context);

	public IGameCardInfo GetCardInfoTarget(IResolutionContext context);

	public IPlayer GetPlayerTarget(IResolutionContext context);

	public IStackable GetStackableTarget(IResolutionContext context);
}