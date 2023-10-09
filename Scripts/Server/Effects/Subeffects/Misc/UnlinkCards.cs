using System.Threading.Tasks;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class UnlinkCards : ServerSubeffect
	{
		public int cardLinkIndex = -1;

		public override Task<ResolutionInfo> Resolve()
		{
			ServerEffect.DestroyCardLink(cardLinkIndex);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}