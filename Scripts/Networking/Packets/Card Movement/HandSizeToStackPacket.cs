using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class HandSizeToStackPacket : Packet
	{
		public int controllerIndex;

		public HandSizeToStackPacket() : base(HandSizeToStack) { }

		public HandSizeToStackPacket(int controllerIndex) : this()
		{
			this.controllerIndex = controllerIndex;
		}

		public override Packet Copy() => new HandSizeToStackPacket(controllerIndex);

		public override Packet? GetInversion(bool known = true) => new HandSizeToStackPacket(1 - controllerIndex);
	}
}

namespace Kompas.Client.Networking
{
	public class HandSizeToStackClientPacket : HandSizeToStackPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			//throw new System.NotImplementedException();
			/*
			var controller = clientGame.Players[controllerIndex];
			clientGame.clientEffectsCtrl.Add(new ClientHandSizeStackable(controller));
			*/
		}
	}
}