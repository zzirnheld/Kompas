using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Effects.Models.Restrictions
{
	public interface IPlayRestriction : IRestriction<(Space s, IPlayer p)>
	{
		public static IPlayRestriction CreateDefault() => new Play.PlayRestriction();

		public bool IsRecommendedNormalPlay((Space s, IPlayer p) item);
		public bool IsRecommendedPlay((Space s, IPlayer p) item, IResolutionContext context);

		public bool IsValidIgnoringAdjacency((Space s, IPlayer p) item, IResolutionContext context);
	}
}