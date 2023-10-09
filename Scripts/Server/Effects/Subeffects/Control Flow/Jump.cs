using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class Jump : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			//this will always jump to the given subeffect index
			return Task.FromResult(ResolutionInfo.Index(JumpIndex));
		}
	}
}