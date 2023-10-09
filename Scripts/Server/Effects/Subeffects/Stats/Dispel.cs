using KompasCore.Cards.Movement;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
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