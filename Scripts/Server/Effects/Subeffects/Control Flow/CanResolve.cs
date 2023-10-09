using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class CanResolve : ServerSubeffect
	{
		public int[] subeffIndices;
		private IEnumerable<ServerSubeffect> Subeffects => subeffIndices.Select(s => ServerEffect.subeffects[s]);

		public int skipIndex = -1;

		public TargetingContext overrideTargetingContext; //If later necessary, make this an array

		public override Task<ResolutionInfo> Resolve()
		{
			var impossible = Subeffects.FirstOrDefault(s => s.IsImpossible(overrideTargetingContext));
			if (impossible == default) return Task.FromResult(ResolutionInfo.Next);
			else
			{
				if (skipIndex == -1) return Task.FromResult(ResolutionInfo.Impossible($"{impossible} could not have resolved."));
				else return Task.FromResult(ResolutionInfo.Index(skipIndex));
			}
		}
	}
}