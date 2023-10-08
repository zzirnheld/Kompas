using System.Threading.Tasks;
using Kompas.Effects.Models;
using Kompas.Server.Gamestate.Players;

namespace Kompas.Server.Effects.Models
{
	public interface IServerStackable : IStackable
	{
		Task StartResolution(IServerResolutionContext context);
	}
}