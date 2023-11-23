using Kompas.Client.Gamestate;
using Kompas.Networking.Packets;

namespace Kompas.Networking.Packets
{
	public class GetTriggerOrderPacket : Packet
	{
		public int?[] sourceCardIds;
		public int[] effIndices;

		public GetTriggerOrderPacket() : base(GetTriggerOrder) { }

		public GetTriggerOrderPacket(int?[] sourceCardIds, int[] effIndices) : this()
		{
			this.sourceCardIds = sourceCardIds;
			this.effIndices = effIndices;
		}

		public override Packet Copy() => new GetTriggerOrderPacket(sourceCardIds, effIndices);
	}
}

namespace Kompas.Client.Networking
{
	public class GetTriggerOrderClientPacket : GetTriggerOrderPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			throw new System.NotImplementedException();
			/*
			List<Trigger> triggers = new();
			for (int i = 0; i < sourceCardIds.Length; i++)
			{
				var card = clientGame.LookupCardByID(sourceCardIds[i]);
				var trigger = card?.Effects.ElementAt(effIndices[i]).Trigger;
				if (trigger != null) triggers.Add(trigger);
			}
			clientGame.clientUIController.effectsUIController.triggerOrderUI.OrderTriggers(triggers);
			*/
		}
	}
}