using System.Threading.Tasks;
using Kompas.Effects.Models;

namespace Kompas.Server.Effects.Models;

/// <summary>
/// Only server effect resolution should contain this sort of StartResolution logic, at least for now.
/// Maybe in the future I'll want to migrate the client to using the same pattern (albeit for visuals)
/// and if I do, I can consider unifying more stuff in EffectStack again.
/// For now, though... eh.
/// </summary>
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