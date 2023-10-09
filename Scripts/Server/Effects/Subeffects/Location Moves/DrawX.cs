using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class DrawX : ServerSubeffect
	{
		protected virtual int ToDraw => Count;

		public override Task<ResolutionInfo> Resolve()
		{
			var drawn = ServerGame.DrawX(PlayerTarget, ToDraw, Effect);
			if (drawn.Count < ToDraw) return Task.FromResult(ResolutionInfo.Impossible(CouldntDrawAllX));
			else return Task.FromResult(ResolutionInfo.Next);
		}
	}
}