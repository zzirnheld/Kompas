using System.Threading.Tasks;
using Kompas.Gamestate.Exceptions;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetEnemy : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			var player = PlayerTarget ?? throw new NullPlayerException("No player to get the enemy of");
			Effect.playerTargets.Add(player.Enemy);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}
