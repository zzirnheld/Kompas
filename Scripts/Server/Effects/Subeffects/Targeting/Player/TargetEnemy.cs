using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetEnemy : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			Effect.playerTargets.Add(PlayerTarget.Enemy);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}
