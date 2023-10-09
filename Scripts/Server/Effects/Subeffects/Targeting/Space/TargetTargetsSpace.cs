using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetTargetsSpace : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget.Location != CardLocation.Board)
				return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

			Effect.AddSpace(CardTarget.Position.Copy);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}