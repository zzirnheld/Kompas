using Kompas.Effects.Models.Restrictions;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class ConditionalJump : ServerSubeffect
	{
		#nullable disable
		[JsonProperty (Required = Required.Always)]
		public IGamestateRestriction jumpIfTrue;
		#nullable restore

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			jumpIfTrue.Initialize(DefaultInitializationContext);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			if (jumpIfTrue.IsValid(ResolutionContext)) return Task.FromResult(ResolutionInfo.Index(JumpIndex));
			else return Task.FromResult(ResolutionInfo.Next);
		}
	}
}