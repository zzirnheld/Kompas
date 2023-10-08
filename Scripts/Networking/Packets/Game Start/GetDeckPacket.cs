﻿using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class GetDeckPacket : Packet
	{
		public GetDeckPacket() : base(GetDeck) { }

		public override Packet Copy() => new GetDeckPacket();
	}
}

namespace Kompas.Client.Networking
{
	public class GetDeckClientPacket : GetDeckPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
			=> throw new System.NotImplementedException();
			//=> clientGame.clientUIController.connectionUIController.Show(UI.ConnectionUIController.ConnectionState.SelectDeck);
	}
}