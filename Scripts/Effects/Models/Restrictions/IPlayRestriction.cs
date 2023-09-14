using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions
{
	public interface IPlayRestriction : IRestriction<(Space s, Player p)>
	{
		public static IPlayRestriction CreateDefault() => new Play.PlayRestriction();

		public bool IsRecommendedNormalPlay((Space s, Player p) item);
		public bool IsRecommendedPlay((Space s, Player p) item, IResolutionContext context);

		public bool IsValidIgnoringAdjacency((Space s, Player p) item, IResolutionContext context);
	}
}