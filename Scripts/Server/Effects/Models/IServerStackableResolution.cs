using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models;

public interface IServerStackableResolution
{
	public Task StartResolution();
}