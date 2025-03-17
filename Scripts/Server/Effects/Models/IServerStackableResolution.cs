using System.Threading.Tasks;
using Kompas.Effects.Models;

namespace Kompas.Server.Effects.Models;

public interface IServerStackableResolution
	: IResolvingStackable
{
	public Task StartResolution();
}

public interface IServerStackableResolution<out StackableType>
	: IServerStackableResolution,
		IResolvingStackable<StackableType, IServerResolutionContext>
	where StackableType : IServerStackable
{ }