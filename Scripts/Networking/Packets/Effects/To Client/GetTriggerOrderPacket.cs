using System.Collections.Generic;
using System.Linq;
using Kompas.Client.Gamestate;
using Kompas.Client.Gamestate.Players;
using Kompas.Effects.Models;
using Kompas.Networking.Packets;
using Kompas.Shared.Enumerable;

namespace Kompas.Networking.Packets
{
	public class GetTriggerOrderPacket : Packet
	{
		public int?[]? sourceCardIds;
		public int[]? effIndices;

		public GetTriggerOrderPacket() : base(GetTriggerOrder) { }

		public GetTriggerOrderPacket(int?[] sourceCardIds, int[] effIndices) : this()
		{
			this.sourceCardIds = sourceCardIds;
			this.effIndices = effIndices;
		}

		public override Packet Copy() => new GetTriggerOrderPacket()
		{
			sourceCardIds = sourceCardIds,
			effIndices = effIndices
		};
	}
}

namespace Kompas.Client.Networking
{
	public class GetTriggerOrderClientPacket : GetTriggerOrderPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			_ = sourceCardIds ?? throw new System.NullReferenceException("sourceCardIDs");
			_ = effIndices ?? throw new System.NullReferenceException("effIndices");
			IList<Trigger> triggers = new List<Trigger>();
			foreach (var (index, ID) in sourceCardIds.Enumerate())
			{
				if (ID == null) continue;
				var card = clientGame.LookupCardByID(ID.Value);
				var trigger = card?.Effects.ElementAt(effIndices[index]).Trigger;
				if (trigger != null) triggers.Add(trigger);
			}

			//For now, just send off the same order
			clientGame.ClientGameController.Notifier.ChooseTriggerOrder(triggers.Select((t, i) => (t, i)));
		}
	}
}