﻿using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;

namespace Kompas.Networking.Packets
{
	public class AttacksThisTurnPacket : Packet
	{
		public int attackerId;
		public int attacksThisTurn;

		public AttacksThisTurnPacket() : base(AttacksThisTurn) { }

		public AttacksThisTurnPacket(int attackerId, int attacksThisTurn) : this()
		{
			this.attackerId = attackerId;
			this.attacksThisTurn = attacksThisTurn;
		}

		public override Packet Copy() => new AttacksThisTurnPacket(attackerId, attacksThisTurn);
	}
}

namespace Kompas.Client.Networking
{
	public class AttacksThisTurnClientPacket : AttacksThisTurnPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.LookupCardByID(attackerId);
			if (card != null) card.AttacksThisTurn = attacksThisTurn;
		}
	}
}