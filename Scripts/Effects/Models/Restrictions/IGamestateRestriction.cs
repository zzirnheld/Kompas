using Kompas.Cards.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions
{
	/// <summary>
	/// Exists on its own as a version of a "restriction" that doesn't require any context
	/// </summary>
	public interface IGamestateRestriction : IContextInitializeable,
		IRestriction<TriggeringEventContext>, IRestriction<Player>, IRestriction<GameCardBase>, IRestriction<Space>,
		IRestriction<(Space s, Player p)>, IRestriction<int>, IListRestriction
	{
		bool IsValid(IResolutionContext context);
	}
}