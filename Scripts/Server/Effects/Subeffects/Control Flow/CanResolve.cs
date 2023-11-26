using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class CanResolve : ServerSubeffect
	{
		#nullable disable
		[JsonProperty (Required = Required.Always)]
		public int[] subeffIndices;
		#nullable restore
		private IEnumerable<ServerSubeffect> Subeffects => subeffIndices.Select(s => ServerEffect.subeffects[s]);

		[JsonProperty]
		public int skipIndex = int.MinValue;

		[JsonProperty]
		public TargetingContext? overrideTargetingContext; //If later necessary, make this an array

		public override Task<ResolutionInfo> Resolve()
		{
			var impossible = Subeffects.FirstOrDefault(s => s.IsImpossible(overrideTargetingContext));
			if (impossible == default) return Task.FromResult(ResolutionInfo.Next); //nothing was impossible
			else
			{
				if (skipIndex == int.MinValue) return Task.FromResult(ResolutionInfo.Impossible($"{impossible} couldn't've resolved."));
				else return Task.FromResult(ResolutionInfo.Index(skipIndex));
			}
		}
	}
}