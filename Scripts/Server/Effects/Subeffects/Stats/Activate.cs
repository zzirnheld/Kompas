using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class Activate : ServerSubeffect
	{
		public bool activate = true;

		public override Task<ResolutionInfo> Resolve()
		{
			CardTarget.SetActivated(activate, Effect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}