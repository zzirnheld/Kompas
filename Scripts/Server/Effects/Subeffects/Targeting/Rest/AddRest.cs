using System.Linq;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class AddRestSubeffect : CardTarget
	{
		public override Task<ResolutionInfo> Resolve()
		{
			Effect.rest.AddRange(DeterminePossibleTargets());
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}