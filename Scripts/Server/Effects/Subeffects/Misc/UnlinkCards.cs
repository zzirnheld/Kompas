using System.Threading.Tasks;
using Kompas.Effects.Subeffects;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class UnlinkCardsData : SubeffectData
{
	[JsonProperty]
	public int cardLinkIndex = -1;
}

public class UnlinkCards : ServerSubeffect
{
	private readonly int cardLinkIndex;

	public UnlinkCards(UnlinkCardsData data) : base(data)
	{
		cardLinkIndex = data.cardLinkIndex;
	}

	public override Task<ResolutionInfo> Resolve(ServerEffectResolution resolution)
	{
		ServerEffect.DestroyCardLink(cardLinkIndex);
		return Task.FromResult(ResolutionInfo.Next);
	}
}