using System;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Players;
using Kompas.Server.Cards.Controllers;
using Kompas.Server.Cards.Models;
using Kompas.Server.Effects.Models;
using Kompas.Server.Gamestate;
using Newtonsoft.Json;

namespace Kompas.Server.Cards.Loading;

public class ServerCardRepository : GameCardRepository<ServerSerializableCard, ServerEffect, ServerCardController>
{
	public ServerCardRepository()
		: this(IFileLoader.Godot, false)
	{ }

	public ServerCardRepository(IFileLoader fileLoader, bool throwExceptions)
		: base(fileLoader, throwExceptions, null)
	{ }

	public static bool CardNameIsCharacter(string? name)
	{
		if (null == name) return false;
		if (!CardExists(name)) return false;

		var card = JsonConvert.DeserializeObject<SerializableCard>(cardJsons[name],
				new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
		return card?.cardType == 'C';
	}

	public static ServerSubeffect[]? InstantiateServerPartialKeyword(string keyword)
	{
		if (!partialKeywordJsons.ContainsKey(keyword))
		{
			Logger.Err($"No partial keyword json found for {keyword}");
			return System.Array.Empty<ServerSubeffect>();
		}

		return JsonConvert.DeserializeObject<ServerSubeffect[]>(partialKeywordJsons[keyword], CardLoadingSettings);
	}

	protected override ServerCardController GetCardController()
	{
		return new ServerCardController();
	}

	public ServerGameCard InstantiateServerCard(string name, IServerGame game, IPlayer owner, int id, bool isAvatar = false)
	{
		string json = cardJsons[name] ?? throw new System.ArgumentException($"Name {name} not associated with json");

		ServerGameCard ConstructCard(ServerSerializableCard cardInfo, ServerEffect[] effects, ServerCardController ctrl)
			=> ServerGameCard.Create(cardInfo, id, owner, game, ctrl, effects, isAvatar);
		ServerEffect ConstructEffect(EffectData data) => new(data);

		var ret = InstantiateGameCard(json, ConstructCard, ConstructEffect)
			?? throw new InvalidOperationException($"Failed to instantiate {json}");
		game.AddCard(ret);
		return ret;
	}
}