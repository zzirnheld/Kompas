using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class ClearRest : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			Effect.rest.Clear();
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}