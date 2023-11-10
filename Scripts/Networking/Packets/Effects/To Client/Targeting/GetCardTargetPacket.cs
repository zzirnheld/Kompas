﻿using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using Newtonsoft.Json;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models;
using Godot;

namespace Kompas.Networking.Packets
{
	public class GetCardTargetPacket : Packet
	{
		public string sourceCardName;
		public string targetBlurb;
		public int[] potentialTargetIDs;
		public IListRestriction listRestriction;
		public bool list;

		public GetCardTargetPacket() : base(GetCardTarget) { }

		public GetCardTargetPacket(string sourceCardName, string targetBlurb, int[] potentialTargetIDs, IListRestriction listRestriction, bool list) : this()
		{
			this.sourceCardName = sourceCardName;
			this.targetBlurb = targetBlurb;
			this.potentialTargetIDs = potentialTargetIDs;
			this.listRestriction = listRestriction;
			this.list = list;
		}

		public override Packet Copy() => new GetCardTargetPacket(sourceCardName, targetBlurb, potentialTargetIDs, listRestriction, list);
	}
}

namespace Kompas.Client.Networking
{
	public class GetCardTargetClientPacket : GetCardTargetPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			IListRestriction listRestriction = this.listRestriction ?? IListRestriction.SingleElement;
			listRestriction.Initialize(new EffectInitializationContext(game: clientGame, source: default));

			clientGame.ClientGameController.TargetingController.StartSearch(list ? TargetMode.CardTargetList : TargetMode.CardTarget,
				potentialTargetIDs, listRestriction, targetBlurb);
		}
	}
}