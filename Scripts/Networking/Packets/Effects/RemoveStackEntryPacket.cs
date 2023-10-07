using Kompas.Networking.Packets;
using Kompas.Gamestate.Client;

namespace Kompas.Networking.Packets
{
	public class RemoveStackEntryPacket : Packet
	{
		public int indexToRemove;

		public RemoveStackEntryPacket() : base(RemoveStackEntry) { }

		public RemoveStackEntryPacket(int indexToRemove) : this()
		{
			this.indexToRemove = indexToRemove;
		}

		public override Packet Copy() => new RemoveStackEntryPacket(indexToRemove);
	}
}

namespace Kompas.Networking.Client
{
	public class RemoveStackEntryClientPacket : RemoveStackEntryPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame) => clientGame.clientEffectsCtrl.Remove(indexToRemove);
	}
}