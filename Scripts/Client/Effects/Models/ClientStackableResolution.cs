using Kompas.Effects.Models;

namespace Kompas.Client.Effects.Models;

public class ClientStackableResolution<Stackable>
	: ResolvingStackable<Stackable, IResolutionContext>
	where Stackable : IClientStackable
{
	//TODO: later create client resolution contexts
	public ClientStackableResolution(Stackable stackable, IResolutionContext? context = null)
		: base(stackable, context ?? new ResolutionContext(null))
	{ }
}