using System.Linq;
using Godot;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Client.Cards.Controllers;
using Kompas.Client.Cards.Models;
using Kompas.Client.Effects.Models;
using Kompas.Client.Gamestate;
using Kompas.Client.Gamestate.Players;

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

		public ClientGameCard InstantiateClientNonAvatar(string json, ClientPlayer owner, int id, ClientGame game)
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

		/*
		public DeckSelectCardController InstantiateDeckSelectCard(string json, Transform parent, DeckSelectCardController prefab, DeckSelectUIController uiCtrl)
		{
			try
			{
				SerializableCard serializableCard = JsonConvert.DeserializeObject<SerializableCard>(json, cardLoadingSettings);
				DeckSelectCardController card = Instantiate(prefab, parent);
				card.SetInfo(serializableCard, uiCtrl, cardFileNames[serializableCard.cardName]);
				return card;
			}
			catch (System.ArgumentException argEx)
			{
				//Catch JSON parse error
				Debug.LogError($"Failed to load {json}, argument exception with message {argEx.Message}");
				return null;
			}
		}*/
	}
}