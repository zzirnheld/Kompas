using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class AddPips : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			PlayerTarget.Pips += Count;
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}