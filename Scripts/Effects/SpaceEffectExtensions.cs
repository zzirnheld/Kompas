using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;

namespace Kompas.Effects
{
	public static class SpaceEffectExtensions
	{
		public static bool AreConnectedBy(this Space source, Space destination, IRestriction<Space> restriction, IResolutionContext context)
			=> source.IsConnectedTo(destination, s => restriction.IsValid(s, context));
	}
}