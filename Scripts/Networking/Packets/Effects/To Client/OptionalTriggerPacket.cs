using Kompas.Networking.Packets;
using Kompas.Gamestate.Client;
using System.Linq;
using KompasClient.Effects;
using UnityEngine;

namespace Kompas.Networking.Packets
{
	public class OptionalTriggerPacket : Packet
	{
		public int sourceCardId;
		public int effIndex;
		public int x;
		public bool showX;

		public int playerBeingAsked;

		public OptionalTriggerPacket() : base(OptionalTrigger) { }

		public OptionalTriggerPacket(int sourceCardId, int effIndex, int x, bool showX, int playerBeingAsked = 0) : this()
		{
			this.sourceCardId = sourceCardId;
			this.effIndex = effIndex;
			this.x = x;
			this.showX = showX;

			this.playerBeingAsked = playerBeingAsked;
		}

		public override Packet Copy() => new OptionalTriggerPacket(sourceCardId, effIndex, x, showX);

		public override Packet GetInversion(bool known = true) => new OptionalTriggerPacket(sourceCardId, effIndex, x, showX, playerBeingAsked: 1);
	}
}

namespace Kompas.Networking.Client
{
	public class OptionalTriggerClientPacket : OptionalTriggerPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.LookupCardByID(sourceCardId);
			if (card == null)
			{
				Debug.LogWarning($"Could not find card with id {sourceCardId}");
				return;
			}
			if (card.Effects.ElementAt(effIndex).Trigger is not ClientTrigger trigger) return;

			trigger.ClientEffect.ClientController = clientGame.clientPlayers[playerBeingAsked];
			clientGame.clientUIController.effectsUIController.ShowOptionalTrigger(trigger, showX, x);
		}
	}
}