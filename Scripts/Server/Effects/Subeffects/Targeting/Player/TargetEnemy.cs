using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class TargetEnemy : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			Effect.playerTargets.Add(EffectController.Enemy);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}
