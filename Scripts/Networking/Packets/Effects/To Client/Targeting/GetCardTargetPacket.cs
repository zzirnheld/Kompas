using Kompas.Networking.Packets;
using Kompas.Client.Gamestate;
using Newtonsoft.Json;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Models;
using Godot;

namespace Kompas.Networking.Packets
{
	public class GetCardTargetPacket : Packet
	{
		public string sourceCardName;
		public string targetBlurb;
		public int[] potentialTargetIDs;
		public string listRestrictionJson;
		public bool list;

		public GetCardTargetPacket() : base(GetCardTarget) { }

		public GetCardTargetPacket(string sourceCardName, string targetBlurb, int[] potentialTargetIDs, string listRestrictionJson, bool list) : this()
		{
			this.sourceCardName = sourceCardName;
			this.targetBlurb = targetBlurb;
			this.potentialTargetIDs = potentialTargetIDs;
			this.listRestrictionJson = listRestrictionJson;
			this.list = list;
		}

		public override Packet Copy() => new GetCardTargetPacket(sourceCardName, targetBlurb, potentialTargetIDs, listRestrictionJson, list);
	}
}

namespace Kompas.Client.Networking
{
	public class GetCardTargetClientPacket : GetCardTargetPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			IListRestriction listRestriction = null;

			try
			{
				if (string.IsNullOrEmpty(listRestrictionJson)) listRestriction = IListRestriction.SingleElement;
				else listRestriction = JsonConvert.DeserializeObject<IListRestriction>(listRestrictionJson);

				listRestriction.Initialize(new EffectInitializationContext(game: clientGame, source: default));
			}
			catch (System.ArgumentException)
			{
				GD.PushError($"Error loading list restriction from json: {listRestrictionJson}");
			}

			clientGame.ClientGameController.TargetingController.StartSearch(list ? TargetMode.CardTargetList : TargetMode.CardTarget,
				potentialTargetIDs, listRestriction);
		}
	}
}