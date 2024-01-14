using Godot;
using Kompas.Client.Gamestate;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Networking.Packets;
using Newtonsoft.Json;

namespace Kompas.Networking.Packets
{
	public class GetHandSizeChoicesOrderPacket : Packet
	{
		public int[]? cardIDs;
		public string listRestrictionJson = string.Empty;

		public GetHandSizeChoicesOrderPacket() : base(ChooseHandSize) { }

		public GetHandSizeChoicesOrderPacket(int[] cardIDs, string listRestrictionJson) : this()
		{
			this.cardIDs = cardIDs;
			this.listRestrictionJson = listRestrictionJson;
		}

		public override Packet Copy() => new GetHandSizeChoicesOrderPacket()
		{
			cardIDs = cardIDs,
			listRestrictionJson = listRestrictionJson
		};
	}
}

namespace Kompas.Client.Networking
{
	public class GetHandSizeChoicesClientPacket : GetHandSizeChoicesOrderPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			if (cardIDs == null)
			{
				GD.PushError("No cardIDs for getting hand size choices");
				return;
			}
			IListRestriction? listRestriction = JsonConvert.DeserializeObject<IListRestriction>(listRestrictionJson)
				?? throw new System.NullReferenceException("Failed to init");
			listRestriction.Initialize(new EffectInitializationContext(game: clientGame, source: default));

			clientGame.ClientGameController.TargetingController.StartHandSizeSearch(cardIDs, listRestriction);
		}
	}
}