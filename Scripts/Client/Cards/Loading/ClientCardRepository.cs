using System.Linq;
using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Client.Cards.Controllers;
using Kompas.Client.Cards.Models;
using Kompas.Client.Effects.Models;
using Kompas.Client.Gamestate;
using Kompas.Client.Gamestate.Players;
using Kompas.Effects.Models;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Client.Cards.Loading;

public class ClientCardRepository : GameCardRepository<ClientSerializableCard, ClientEffect, ClientCardController>
{
	public ClientCardRepository(PackedScene cardPrefab)
		: this(IFileLoader.Godot, false, cardPrefab)
	{ }

	public ClientCardRepository(IFileLoader fileLoader, bool throwExceptions, PackedScene cardPrefab)
		: base(fileLoader, throwExceptions, cardPrefab)
	{ }

	public ClientGameCard? InstantiateClientAvatar(string json, ClientPlayer owner, int id, ClientGame game)
	{
		void validation(SerializableCard cardInfo)
		{
			if (cardInfo.cardType != 'C') throw new System.NotImplementedException("Card type for client avatar isn't character!");
		}

		ClientGameCard ConstructAvatar(ClientSerializableCard cardInfo, ClientEffect[] effects, ClientCardController ctrl)
			=> ClientGameCard.Create(cardInfo, id, game, owner, effects, ctrl, isAvatar: true);

		return InstantiateGameCard(SanitizeJson(json), ConstructAvatar, ConstructClientEffect, validation);
	}

	private static ClientEffect ConstructClientEffect(EffectData data) => new(data);

	private static string SanitizeJson(string json) => json; //TODO

	public ClientGameCard? InstantiateClientNonAvatar(string json, IPlayer owner, int id, ClientGame game)
	{
		ClientGameCard ConstructNonAvatar(ClientSerializableCard cardInfo, ClientEffect[] effects, ClientCardController ctrl)
			=> ClientGameCard.Create(cardInfo, id, game, owner, effects, ctrl);
		var card = InstantiateGameCard(SanitizeJson(json),
			ConstructNonAvatar,
			ConstructClientEffect);

		if (card == null) return card;

		//handle adding existing card links
		foreach (var c in card.Game.Cards.ToArray())
		{
			foreach (var link in c.CardLinkHandler.Links.ToArray())
			{
				if (link.CardIDs.Contains(id)) card.CardLinkHandler.AddLink(link);
			}
		}

		return card;
	}

	public SelectDeckCard? InstantiateDeckSelectCard(string cardName)
	{
		var json = GetJsonFromName(cardName) ?? throw new System.NullReferenceException($"No json found for {cardName}");
		var fileName = FileNameFor(cardName) ?? throw new System.NullReferenceException($"No file name found for {cardName}");
		return InstantiateDeckSelectCard(json, fileName);
	}

	public SelectDeckCard? InstantiateDeckSelectCard(string json, string fileName)
	{
		try
		{
			var serializableCard = JsonConvert.DeserializeObject<SerializableCard>(SanitizeJson(json), CardLoadingSettings);
			if (serializableCard == null)
			{
				Logger.Err($"Failed to load {json}");
				return null;
			}

			return new SelectDeckCard(serializableCard.Stats, serializableCard.subtext, serializableCard.spellTypes, serializableCard.unique,
				serializableCard.radius, serializableCard.duration, serializableCard.cardType, serializableCard.cardName,
				fileName, //TODO signature that takes in serializablecard, TODO signature in card base for the same, TODO fileName
				serializableCard.effText, serializableCard.subtypeText, this);
		}
		catch (System.ArgumentException argEx)
		{
			//Catch JSON parse error
			Logger.Err($"Failed to load {json}, argument exception with message {argEx.Message}");
			return null;
		}
	}
}