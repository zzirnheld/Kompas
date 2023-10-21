
using System.Collections.Generic;
using Kompas.Cards.Controllers;
using Kompas.Cards.Models;
using Kompas.Client.Cards.Controllers;
using Kompas.Client.Effects.Models;
using Kompas.Client.Gamestate;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;
using Kompas.Shared.Enumerable;

namespace Kompas.Client.Cards.Models
{
	public class ClientGameCard : GameCard
	{
		public ClientGame ClientGame { get; protected set; }
		public override IGame Game => ClientGame;

		public override Location Location
		{
			get => base.Location;
			protected set
			{
				base.Location = value;
				//TODO see if this ends up still being necessary
				//ClientGame.clientUIController.Leyload = Game.Leyload;
				if (CardController != null)
				{
					//CardController.gameCardViewController.Refresh();
					UpdateRevealed();
				}
			}
		}

		private readonly bool isAvatar;
		public override bool IsAvatar => isAvatar;

		public ClientEffect[] ClientEffects { get; }
		public override IReadOnlyCollection<Effect> Effects => ClientEffects;
		public ClientCardController ClientCardController { get; }
		public override ICardController CardController => ClientCardController;

		private bool knownToEnemy = false;
		public override bool KnownToEnemy
		{
			get => knownToEnemy;
			set
			{
				knownToEnemy = value;
				UpdateRevealed();
			}
		}

		private ClientGameCard(SerializableCard serializedCard, int id, ClientGame game,
			IPlayer owningPlayer, ClientEffect[] effects, ClientCardController cardController, bool isAvatar)
			: base (serializedCard, id, owningPlayer)
		{
			//TODO: game should add card after creating it
			//owner.Game.AddCard(this);

			ClientCardController = cardController;
			ClientGame = game;
			ClientEffects = effects;

			this.isAvatar = isAvatar;

			//cardController.gameCardViewController.Focus(this);
		}

		/// <summary>
        /// Factory method to create card and initialize the relevant things with a non-leaked this instance
        /// </summary>
		public static ClientGameCard Create(SerializableCard serializedCard, int id, ClientGame game,
			IPlayer owningPlayer, ClientEffect[] effects, ClientCardController cardController, bool isAvatar = false)
		{
			ClientGameCard ret = new(serializedCard, id, game, owningPlayer, effects, cardController, isAvatar);

			cardController.Card = ret;
			foreach (var (index, eff) in effects.Enumerate()) eff.SetInfo(ret, game, index, owningPlayer);
			game.AddCard(ret);

			return ret;
		}

		public override void Remove(IStackable stackSrc = null)
		{
			//TODO GameController
			//ClientGame.MarkCardDirty(this);
			base.Remove(stackSrc);
		}

		public override void SetN(int n, IStackable stackSrc = null, bool notify = true)
		{
			base.SetN(n, stackSrc, notify);
			//TODO refresh space indicators for spaces can move
			//ClientGame?.clientUIController.CardViewController.Refresh();
		}

		private void UpdateRevealed() => ClientCardController.RefreshRevealed();
	}
}