using Kompas.Cards.Models;
using Kompas.Effects.Models.Identities;
using Kompas.Effects.Models.Identities.Cards;
using Newtonsoft.Json;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetOtherInFightData : AutoTargetCardIdentityData
{
	[JsonProperty]
	public IIdentity<IGameCardInfo> other = new TargetIndex();
}

public class TargetOtherInFight : AutoTargetCardIdentity
{
	public TargetOtherInFight(TargetOtherInFightData data) : base(PopulateIdentity(data)) { }

	private static AutoTargetCardIdentityData PopulateIdentity(TargetOtherInFightData data)
	{
		data.subeffectCardIdentity = new OtherInFight() { other = data.other };
		return data;
	}
}