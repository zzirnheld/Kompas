using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class SetAvatarPacket : Packet
	{
		public int playerIndex;
		public string json = string.Empty;
		public int cardId;

		public SetAvatarPacket() : base(SetAvatar) { }

		public SetAvatarPacket(int playerIndex, string json, int cardId) : this()
		{
			this.playerIndex = playerIndex;
			this.json = json;
			this.cardId = cardId;
		}

		public override Packet Copy() => new SetAvatarPacket();

		public override Packet? GetInversion(bool known = true)
			=> new SetAvatarPacket(1 - playerIndex, json, cardId);
	}
}

namespace Kompas.Client.Networking
{
	public class SetAvatarClientPacket : SetAvatarPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame) => clientGame.SetAvatar(playerIndex, json, cardId);
	}
}