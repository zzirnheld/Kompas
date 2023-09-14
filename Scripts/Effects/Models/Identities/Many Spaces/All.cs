using System.Collections.Generic;
using System.Linq;
using Kompas.Gamestate;

namespace Kompas.Effects.Models.Identities.ManySpaces
{
	public class All : ContextlessLeafIdentityBase<IReadOnlyCollection<Space>>
	{
		protected override IReadOnlyCollection<Space> AbstractItem => Space.Spaces.ToList();
	}
}