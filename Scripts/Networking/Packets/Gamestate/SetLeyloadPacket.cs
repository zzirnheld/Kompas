using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;

namespace Kompas.Networking.Packets
{
	public class SetLeyloadPacket : Packet
	{
		public int leyload;

		public SetLeyloadPacket() : base(SetLeyload) { }

		public SetLeyloadPacket(int leyload) : this()
		{
			this.leyload = leyload;
		}

		public override Packet Copy() => new SetLeyloadPacket(leyload);
	}
}

namespace Kompas.Client.Networking
{
	public class SetLeyloadClientPacket : SetLeyloadPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame) => clientGame.Leyload = leyload;
	}
}