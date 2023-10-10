﻿using Kompas.Effects.Models.Identities.Cards;

namespace Kompas.Server.Effects.Models.Subeffects
{
	public class TargetAvatar : AutoTargetCardIdentity
	{
		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			subeffectCardIdentity = new Avatar() { player = new Kompas.Effects.Models.Identities.Players.TargetIndex() };
			base.Initialize(eff, subeffIndex);
		}
	}
}