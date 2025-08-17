using Kompas.Effects.Models.Identities.Cards;

namespace Kompas.Server.Effects.Models.Subeffects;

public class TargetAvatarData : AutoTargetCardIdentityData { }

public class TargetAvatar : AutoTargetCardIdentity
{
	public TargetAvatar(TargetAvatarData data) : base(PopulateIdentity(data)) { }

	private static AutoTargetCardIdentityData PopulateIdentity(TargetAvatarData data)
	{
		data.subeffectCardIdentity = new Avatar() { player = new Kompas.Effects.Models.Identities.Players.TargetIndex() };
		return data;
	}
}