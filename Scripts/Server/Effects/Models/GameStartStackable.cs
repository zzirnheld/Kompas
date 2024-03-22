using System.Threading.Tasks;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Server.Effects.Models
{
	public class GameStartStackable : IStackable, IServerStackable
	{
		public GameCard? Card => null;

		public IPlayer? ControllingPlayer => null;

		public GameCard? GetCause(IGameCardInfo? withRespectTo) => Card;

		public Task StartResolution(IServerResolutionContext context)
		{
			throw new System.NotImplementedException();
		}
	}
}