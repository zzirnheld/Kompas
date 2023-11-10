using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Server.Cards.Controllers;
using Kompas.Server.Cards.Models;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate;
using Kompas.Server.Gamestate.Players;
using Kompas.Shared.Enumerable;
using Newtonsoft.Json;

namespace Kompas.Server.Cards.Loading
{
	public class ServerCardRepository : GameCardRepository<ServerSerializableCard, ServerEffect, ServerCardController>
	{
		public ServerCardRepository(PackedScene cardPrefab)
			: base(cardPrefab)
		{
		}

		public static bool CardNameIsCharacter(string name)
		{
			if (!CardExists(name)) return false;

			var card = JsonConvert.DeserializeObject<SerializableCard>(cardJsons[name],
					new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
			return card.cardType == 'C';
		}

		public static ServerSubeffect[] InstantiateServerPartialKeyword(string keyword)
		{
			if (!partialKeywordJsons.ContainsKey(keyword))
			{
				GD.PrintErr($"No partial keyword json found for {keyword}");
				return System.Array.Empty<ServerSubeffect>();
			}

			return JsonConvert.DeserializeObject<ServerSubeffect[]>(partialKeywordJsons[keyword], CardLoadingSettings);
		}

		protected override ServerCardController GetCardController()
		{
			return new ServerCardController();
		}

		public ServerGameCard InstantiateServerCard(string name, ServerGame game, ServerPlayer owner, int id, bool avatar = false)
		{
			string json = cardJsons[name] ?? throw new System.ArgumentException($"Name {name} not associated with json");

			ServerGameCard ConstructCard(ServerSerializableCard cardInfo, ServerEffect[] effects, ServerCardController ctrl)
			{
				var ret = new ServerGameCard(cardInfo, id, owner, game, ctrl, effects, avatar);
				foreach (var (index, eff) in effects.Enumerate()) eff.SetInfo(ret, game, index);
				return ret;
			}
			var ret = InstantiateGameCard(json, ConstructCard);
			game.AddCard(ret);
			return ret;
		}
	}
}