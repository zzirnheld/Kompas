using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class DeleteTarget : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			RemoveTarget();
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}