using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class Negate : ServerSubeffect
	{
		public bool negated = true;

		public override Task<ResolutionInfo> Resolve()
		{
			CardTarget.SetNegated(negated, ServerEffect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}