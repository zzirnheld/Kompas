using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
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