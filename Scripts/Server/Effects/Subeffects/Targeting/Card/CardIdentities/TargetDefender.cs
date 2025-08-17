using Kompas.Effects.Models.Identities.Cards;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetDefenderData : AutoTargetCardIdentityData { }

public class TargetDefender : AutoTargetCardIdentity
{
	public TargetDefender(TargetDefenderData data) : base(PopulateIdentity(data)) { }

	private static AutoTargetCardIdentityData PopulateIdentity(TargetDefenderData data)
	{
		data.subeffectCardIdentity = new Defender();
		return data;
	}
}