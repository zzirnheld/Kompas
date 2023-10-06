using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using System.Linq;
using UnityEngine;

namespace Kompas.Networking.Packets
{
	public class SetEffectsXPacket : Packet
	{
		public int sourceCardId;
		public int effIndex;
		public int x;

		public SetEffectsXPacket() : base(SetEffectsX) { }

		public SetEffectsXPacket(int sourceCardId, int effIndex, int x) : this()
		{
			this.sourceCardId = sourceCardId;
			this.effIndex = effIndex;
			this.x = x;
		}

		public override Packet Copy() => new SetEffectsXPacket(sourceCardId, effIndex, x);

		public override Packet GetInversion(bool known = true) => new SetEffectsXPacket(sourceCardId, effIndex, x);
	}
}

namespace Kompas.Client.Networking
{
	public class SetEffectsXClientPacket : SetEffectsXPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.LookupCardByID(sourceCardId);
			if (card != null)
			{
				Debug.Log($"{card} ;;;;\n {card?.Effects} ;;;;\n {card?.Effects?.ElementAt(effIndex)}");
				card.Effects.ElementAt(effIndex).X = x;
			}
		}
	}
}