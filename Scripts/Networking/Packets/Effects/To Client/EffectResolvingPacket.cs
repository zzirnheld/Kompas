using Kompas.Networking.Packets;
using Kompas.Gamestate.Client;
using System.Linq;
using KompasClient.Effects;

namespace Kompas.Networking.Packets
{
	public class EffectResolvingPacket : Packet
	{
		public int sourceCardId;
		public int effIndex;
		public int controllerIndex;

		public EffectResolvingPacket() : base(EffectResolving) { }

		public EffectResolvingPacket(int sourceCardId, int effIndex, int controllerIndex, bool invert = false) : this()
		{
			this.sourceCardId = sourceCardId;
			this.effIndex = effIndex;
			this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
		}

		public override Packet Copy() => new EffectResolvingPacket(sourceCardId, effIndex, controllerIndex);

		public override Packet GetInversion(bool known = true) => new EffectResolvingPacket(sourceCardId, effIndex, controllerIndex, invert: true);
	}
}

namespace Kompas.Networking.Client
{
	public class EffectResolvingClientPacket : EffectResolvingPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.LookupCardByID(sourceCardId);
			if (card == null) return;
			var eff = card.Effects.ElementAt(effIndex) as ClientEffect;
			eff.Controller = clientGame.Players[controllerIndex];
			eff.StartResolution(default); //TODO eventually make this be real
		}
	}
}