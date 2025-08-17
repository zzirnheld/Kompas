using Kompas.Effects.Models.Identities.Cards;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetThisData : AutoTargetCardIdentityData { }

public class TargetThis : AutoTargetCardIdentity
{
	public TargetThis(TargetThisData data) : base(PopulateIdentity(data)) { }

	private static AutoTargetCardIdentityData PopulateIdentity(TargetThisData data)
	{
		data.subeffectCardIdentity = new ThisCardNow();
		return data;
	}
}