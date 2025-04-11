using Kompas.Effects.Models;

namespace Kompas.Client.Effects.Models;

public class ClientStackableResolution<Stackable>
	: StackableResolution<Stackable, IResolutionContext>
	where Stackable : IClientStackable
{
	//TODO: later create client resolution contexts
	public ClientStackableResolution(Stackable stackable, string blurb, IResolutionContext? context = null)
		: base(stackable, context ?? new ResolutionContext(null, blurb))
	{ }
}