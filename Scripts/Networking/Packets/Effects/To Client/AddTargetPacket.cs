using Kompas.Client.Effects.Models;
using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;
using System.Linq;

namespace Kompas.Networking.Packets
{
	public class AddTargetPacket : Packet
	{
		public int sourceCardId;
		public int effIndex;
		public int targetCardId;

		public AddTargetPacket() : base(AddTarget) { }

		public AddTargetPacket(int sourceCardId, int effIndex, int targetCardId) : this()
		{
			this.sourceCardId = sourceCardId;
			this.effIndex = effIndex;
			this.targetCardId = targetCardId;
		}

		public override Packet Copy() => new AddTargetPacket(sourceCardId, effIndex, targetCardId);
	}
}

namespace Kompas.Client.Networking
{
	public class AddTargetClientPacket : AddTargetPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var source = clientGame.LookupCardByID(sourceCardId);
			var target = clientGame.LookupCardByID(targetCardId);
			//TODO check when PR: is there something that ClientEffect used to do?
			//if not, should still hook into something now... for visuals.
			// if (source != null && target != null) source.Effects.ElementAt(effIndex) as ClientEffect.AddTarget(target);
		}
	}
}