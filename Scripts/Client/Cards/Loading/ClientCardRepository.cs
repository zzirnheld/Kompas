using System.Linq;
using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Client.Cards.Controllers;
using Kompas.Client.Cards.Models;
using Kompas.Client.Effects.Models;
using Kompas.Client.Gamestate;
using Kompas.Client.Gamestate.Players;
using Kompas.Gamestate.Players;
using Newtonsoft.Json;

namespace Kompas.Client.Cards.Loading
{
	public class ClientCardRepository : GameCardRepository<ClientSerializableCard, ClientEffect, ClientCardController>
	{
		//public GameObject DeckSelectCardPrefab;

		public Material friendlyCardMaterial;
		public Material enemyCardMaterial;

		public ClientGameCard InstantiateClientAvatar(string json, ClientPlayer owner, int id, ClientGame game)
		{
			void validation(SerializableCard cardInfo)
			{
				if (cardInfo.cardType != 'C') throw new System.NotImplementedException("Card type for client avatar isn't character!");
			}

			ClientGameCard ConstructAvatar(ClientSerializableCard cardInfo, ClientEffect[] effects, ClientCardController ctrl)
				=> new(cardInfo, id, game, owner, effects, ctrl, isAvatar: true);

			return InstantiateGameCard(json, ConstructAvatar, validation);
		}

		public ClientGameCard InstantiateClientNonAvatar(string json, IPlayer owner, int id, ClientGame game)
		{
			var card = InstantiateGameCard(json,
				(cardInfo, effects, ctrl) => new ClientGameCard(cardInfo, id, game, owner, effects, ctrl));

			//TODO set materials - should happen elsewhere based on ClientSettings? or maybe as a callback after controller is instantiated
			/*
			card.ClientCardController.gameCardViewController.cardModelController.SetFrameMaterial(owner.Friendly ? friendlyCardMaterial : enemyCardMaterial);
			card.ClientCardController.gameCardViewController.Refresh();
			*/

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

		public SelectDeckCard InstantiateDeckSelectCard(string cardName)
			=> InstantiateDeckSelectCard(GetJsonFromName(cardName), FileNameFor(cardName));

		public SelectDeckCard InstantiateDeckSelectCard(string json, string fileName)
		{
			try
			{
				SerializableCard serializableCard = JsonConvert.DeserializeObject<SerializableCard>(json, CardLoadingSettings);
				return new SelectDeckCard(serializableCard.Stats, serializableCard.subtext, serializableCard.spellTypes, serializableCard.unique,
					serializableCard.radius, serializableCard.duration, serializableCard.cardType, serializableCard.cardName,
					fileName, //TODO signature that takes in serializablecard, TODO signature in card base for the same, TODO fileName
					serializableCard.effText, serializableCard.subtypeText);
			}
			catch (System.ArgumentException argEx)
			{
				//Catch JSON parse error
				GD.PrintErr($"Failed to load {json}, argument exception with message {argEx.Message}");
				return null;
			}
		}
	}
}