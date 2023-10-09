using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Effects.Subeffects;
using Kompas.Server.Cards.Controllers;
using Kompas.Server.Cards.Models;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Server.Cards.Loading
{
	public class ServerCardRepository : GameCardRepository<ServerSerializableCard, ServerEffect, ServerCardController>
	{
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

		public ServerGameCard InstantiateServerCard(string name, ServerPlayer owner, int id, bool avatar = false)
		{
			string json = cardJsons[name] ?? throw new System.ArgumentException($"Name {name} not associated with json");
			return InstantiateGameCard<ServerGameCard>(json, (cardInfo, effects, ctrl) => new ServerGameCard(cardInfo, id, ctrl, owner, effects, avatar));
		}
	}
}