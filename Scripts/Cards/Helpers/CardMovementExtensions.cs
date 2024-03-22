using Kompas.Cards.Models;
using Kompas.Client.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions.Cards;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Cards.Movement
{
	public static class GameCardMovementExtensions
	{
		public static void Discard<CardType, PlayerType>(this CardType card, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
			=> card.ControllingPlayer.Discard.Add(card, stackableCause: stackSrc);

		public static void Hand<CardType, PlayerType>(this CardType card, PlayerType controllingPlayer, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
			=> controllingPlayer.Hand.Add(card, stackableCause: stackSrc);
		public static void Rehand<CardType, PlayerType>(this CardType card, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
			=> card.Hand(card.OwningPlayer, stackSrc);

		public static void Reshuffle<CardType, PlayerType>(this CardType card, PlayerType controllingPlayer, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
			=> controllingPlayer.Deck.ShuffleIn(card, stackSrc);
		public static void Reshuffle<CardType, PlayerType>(this CardType card, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
			=> card.Reshuffle(card.OwningPlayer, stackSrc);

		public static void Topdeck<CardType, PlayerType>(this CardType card, PlayerType controllingPlayer, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
			=> controllingPlayer.Deck.PushTopdeck(card, stackSrc);
		public static void Topdeck<CardType, PlayerType>(this CardType card, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
			=> card.Topdeck(card.OwningPlayer, stackSrc);

		public static void Bottomdeck<CardType, PlayerType>(this CardType card, PlayerType controllingPlayer, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
			=> controllingPlayer.Deck.PushBottomdeck(card, stackSrc);
		public static void Bottomdeck<CardType, PlayerType>(this CardType card, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
			=> card.Bottomdeck(card.OwningPlayer, stackSrc);

		public static void Annihilate<CardType, PlayerType>(this CardType card, PlayerType controllingPlayer, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
			=> controllingPlayer.Annihilation.Add(card, stackableCause: stackSrc);
		public static void Annihilate<CardType, PlayerType>(this CardType card, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
			=> card.Annihilate(card.OwningPlayer, stackSrc);

		//If you're looking for this, you'll need to do it more manually by the client/server version of the thing, to maintain the knowledge of what kind of controller you need
		public static void Play<CardType, PlayerType>(this CardType card, Space to, PlayerType controllingPlayer, IStackable? stackSrc = null, bool payCost = false)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
		{
			//TODO move this to server-side
			//var costToPay = card.Cost;
			card.Game.Board.Play(card, to, controllingPlayer, stackSrc);

			//if (payCost) controllingPlayer.Pips -= costToPay;
		}

		public static void Move<CardType, PlayerType>(this CardType card, Space to, bool normalMove, PlayerType? mover, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
			=> card.Game.Board.Move(card, to, normalMove, mover, stackSrc);

		public static void Dispel<CardType, PlayerType>(this CardType card, IStackable? stackSrc = null)
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
		{
			card.SetNegated(true, stackSrc);
			card.Discard<CardType, PlayerType>(stackSrc);
		}
	}
}