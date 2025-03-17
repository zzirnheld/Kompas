using System.Threading.Tasks;
using Kompas.Effects.Models;

namespace Kompas.Server.Effects.Models;

public interface IServerStackableResolution
	: IStackableResolution
{
	public Task StartResolution();
}

public interface IServerStackableResolution<out StackableType>
	: IServerStackableResolution,
		IStackableResolution<StackableType, IServerResolutionContext>
	where StackableType : IServerStackable
{ }