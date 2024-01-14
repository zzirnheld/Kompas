using System.Threading.Tasks;
using Kompas.Gamestate.Players;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TakeControl : ServerSubeffect
	{
		public int ControllerIndexOffset = 0;

		//TODO abstract this logic into a parent class with other player offset things
		private IPlayer NewController => Game.Players[(PlayerTarget.Index + ControllerIndexOffset) % Game.Players.Length];

		public override Task<ResolutionInfo> Resolve()
		{
			CardTarget.ControllingPlayer = NewController;
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}