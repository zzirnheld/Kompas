using Kompas.Effects.Models.Restrictions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class ConditionalEnd : ServerSubeffect
	{
		#nullable disable
		[JsonProperty (Required = Required.Always)]
		public IGamestateRestriction endIfTrue;
		#nullable restore

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			endIfTrue.Initialize(DefaultInitializationContext);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			//TODO implement a ToHumanReadableString sort of thing to provide as a reason here
			if (endIfTrue.IsValid(ResolutionContext)) return Task.FromResult(ResolutionInfo.End("I said so"));
			else return Task.FromResult(ResolutionInfo.Next);
		}
	}
}