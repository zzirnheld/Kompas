using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class TargetTriggeringStackableSource : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			ServerEffect.AddTarget(ResolutionContext.TriggerContext?.stackableCause?.Source
				??  throw new KompasException("Null stackable", string.Empty));
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}