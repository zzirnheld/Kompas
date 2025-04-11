using System.Threading.Tasks;
using Kompas.Effects.Models;

namespace Kompas.Server.Effects.Models;

public interface IServerStackable : IStackable
{
	// Task StartResolution(IServerResolutionContext context);
}