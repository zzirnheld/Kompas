using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;
using System.Linq;

namespace Kompas.Networking.Packets
{
	public class RemoveTargetPacket : Packet
	{
		public int sourceCardId;
		public int effIndex;
		public int targetCardId;

		public RemoveTargetPacket() : base(RemoveTarget) { }

		public RemoveTargetPacket(int sourceCardId, int effIndex, int targetCardId) : this()
		{
			this.sourceCardId = sourceCardId;
			this.effIndex = effIndex;
			this.targetCardId = targetCardId;
		}

		public override Packet Copy() => new RemoveTargetPacket(sourceCardId, effIndex, targetCardId);
	}
}

namespace Kompas.Client.Networking
{
	public class RemoveTargetClientPacket : RemoveTargetPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var source = clientGame.LookupCardByID(sourceCardId);
			var target = clientGame.LookupCardByID(targetCardId);
			if (source != null && target != null) source.Effects.ElementAt(effIndex)?.RemoveTarget(target);
		}
	}
}