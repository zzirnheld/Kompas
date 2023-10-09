using Kompas.Cards.Movement;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class Dispel : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			CardTarget.Dispel(Effect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}