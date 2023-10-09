using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetThisSpace : ServerSubeffect
	{
		public override Task<ResolutionInfo> Resolve()
		{
			if (ThisCard.Location != Location.Board)
				return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

			ServerEffect.AddSpace(ThisCard.Position);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}