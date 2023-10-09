using System.Threading.Tasks;
using Kompas.Gamestate;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetTurnPlayer : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			Effect.playerTargets.Add(Game.TurnPlayer);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}
