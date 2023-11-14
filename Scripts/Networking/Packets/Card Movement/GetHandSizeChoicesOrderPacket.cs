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
		public int[] cardIDs;
		public string listRestrictionJson;

		public GetHandSizeChoicesOrderPacket() : base(ChooseHandSize) { }

		public GetHandSizeChoicesOrderPacket(int[] cardIDs, string listRestrictionJson) : this()
		{
			this.cardIDs = cardIDs;
			this.listRestrictionJson = listRestrictionJson;
		}

		public override Packet Copy() => new GetHandSizeChoicesOrderPacket(cardIDs, listRestrictionJson);
	}
}

namespace Kompas.Client.Networking
{
	public class GetHandSizeChoicesClientPacket : GetHandSizeChoicesOrderPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			IListRestriction listRestriction = JsonConvert.DeserializeObject<IListRestriction>(listRestrictionJson);
			listRestriction.Initialize(new EffectInitializationContext(game: clientGame, source: default));

			clientGame.ClientGameController.TargetingController.StartHandSizeSearch(cardIDs, listRestriction);
		}
	}
}