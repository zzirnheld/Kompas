using Kompas.Gamestate.Exceptions;
using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetTriggeringStackableSource : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			ServerEffect.AddTarget(ResolutionContext.TriggerContext?.stackableCause?.Card
				??  throw new KompasException("Null stackable", string.Empty));
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}