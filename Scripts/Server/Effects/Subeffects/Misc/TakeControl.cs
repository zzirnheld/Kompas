using System.Threading.Tasks;

namespace Kompas.Server.Effects.Subeffects
{
	public class TakeControl : ServerSubeffect
	{
		public int ControllerIndexOffset = 0;

		//TODO abstract this logic into a parent class with other player offset things
		private Player NewController => ServerGame.Players[(EffectController.index + ControllerIndexOffset) % ServerGame.Players.Length];

		public override Task<ResolutionInfo> Resolve()
		{
			CardTarget.Controller = NewController;
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}