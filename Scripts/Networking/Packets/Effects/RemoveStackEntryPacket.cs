using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;

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

namespace Kompas.Client.Networking
{
	public class RemoveStackEntryClientPacket : RemoveStackEntryPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame) { }// => throw new System.NotImplementedException(); // clientGame.clientEffectsCtrl.Remove(indexToRemove);
	}
}