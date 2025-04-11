using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using System.Linq;
using Kompas.Client.Effects.Models;

namespace Kompas.Networking.Packets
{
	public class EffectActivatedPacket : Packet
	{
		public int cardID;
		public int effectIndex;
		#nullable disable
		public string blurb;
		#nullable enable

		public EffectActivatedPacket() : base(EffectActivated) { }

		public EffectActivatedPacket(int cardID, int effectIndex, string blurb) : this()
		{
			this.cardID = cardID;
			this.effectIndex = effectIndex;
			this.blurb = blurb;
		}

		public override Packet Copy() => new EffectActivatedPacket(cardID, effectIndex, blurb);

		public override Packet? GetInversion(bool known = true) => new EffectActivatedPacket(cardID, effectIndex, blurb);
	}
}

namespace Kompas.Client.Networking
{
	public class EffectActivatedClientPacket : EffectActivatedPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.LookupCardByID(cardID);
			var effect = card?.Effects.ElementAt(effectIndex);
			if (effect is not ClientEffect eff)
			{
				Logger.Err($"Couldn't find effect {effectIndex} of {cardID}");
				return;
			}

			clientGame.StackController.Activated(eff, blurb);
		}
	}
}