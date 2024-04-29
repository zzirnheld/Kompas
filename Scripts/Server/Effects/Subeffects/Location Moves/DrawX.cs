using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class DrawX : ServerSubeffect
	{
		protected virtual int ToDraw => Count;

		[JsonProperty]
		public bool addAsTarget = false;

		public override Task<ResolutionInfo> Resolve()
		{
			var drawn = ServerGame.DrawX(PlayerTarget, ToDraw, Effect);
			if (addAsTarget) foreach (var card in drawn) Effect.AddTarget(card);

			if (drawn.Count < ToDraw) return Task.FromResult(ResolutionInfo.Impossible(CouldntDrawAllX));
			else return Task.FromResult(ResolutionInfo.Next);
		}
	}
}