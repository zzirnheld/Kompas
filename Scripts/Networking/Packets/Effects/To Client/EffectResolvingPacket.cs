using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using System.Linq;
using Kompas.Client.Effects;
using Kompas.Client.Effects.Models;
using System;

namespace Kompas.Networking.Packets
{
	public class EffectResolvingPacket : Packet
	{
		public int cardID;
		public int effectIndex;
		public int controllerIndex;

		public EffectResolvingPacket() : base(EffectResolving) { }

		public EffectResolvingPacket(int sourceCardId, int effIndex, int controllerIndex, bool invert = false) : this()
		{
			this.cardID = sourceCardId;
			this.effectIndex = effIndex;
			this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
		}

		public override Packet Copy() => new EffectResolvingPacket(cardID, effectIndex, controllerIndex);

		public override Packet? GetInversion(bool known = true) => new EffectResolvingPacket(cardID, effectIndex, controllerIndex, invert: true);
	}
}

namespace Kompas.Client.Networking
{
	public class EffectResolvingClientPacket : EffectResolvingPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.LookupCardByID(cardID);
			if (card == null) return;
			var eff = card.Effects.ElementAt(effectIndex) as ClientEffect
				?? throw new InvalidOperationException("Effect on client wasn't a ClientEffect!");
			clientGame.StackController.Resolve(eff);
		}
	}
}