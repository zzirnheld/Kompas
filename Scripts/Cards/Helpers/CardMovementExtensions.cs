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
		public static void Discard<TCard, TPlayer>(this TCard card, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> card.ControllingPlayer.Discard.Add(card, stackableCause: stackSrc);

		public static void Hand<TCard, TPlayer>(this TCard card, TPlayer controllingPlayer, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> controllingPlayer.Hand.Add(card, stackableCause: stackSrc);
		public static void Rehand<TCard, TPlayer>(this TCard card, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> card.Hand(card.OwningPlayer, stackSrc);

		public static void Reshuffle<TCard, TPlayer>(this TCard card, TPlayer controllingPlayer, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> controllingPlayer.Deck.ShuffleIn(card, stackSrc);
		public static void Reshuffle<TCard, TPlayer>(this TCard card, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> card.Reshuffle(card.OwningPlayer, stackSrc);

		public static void Topdeck<TCard, TPlayer>(this TCard card, TPlayer controllingPlayer, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> controllingPlayer.Deck.PushTopdeck(card, stackSrc);
		public static void Topdeck<TCard, TPlayer>(this TCard card, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> card.Topdeck(card.OwningPlayer, stackSrc);

		public static void Bottomdeck<TCard, TPlayer>(this TCard card, TPlayer controllingPlayer, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> controllingPlayer.Deck.PushBottomdeck(card, stackSrc);
		public static void Bottomdeck<TCard, TPlayer>(this TCard card, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> card.Bottomdeck(card.OwningPlayer, stackSrc);

		public static void Annihilate<TCard, TPlayer>(this TCard card, TPlayer controllingPlayer, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> controllingPlayer.Annihilation.Add(card, stackableCause: stackSrc);
		public static void Annihilate<TCard, TPlayer>(this TCard card, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> card.Annihilate(card.OwningPlayer, stackSrc);

		//If you're looking for this, you'll need to do it more manually by the client/server version of the thing, to maintain the knowledge of what kind of controller you need
		public static void Play<TCard, TPlayer>(this TCard card, Space to, TPlayer controllingPlayer, IStackable? stackSrc = null, bool payCost = false)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
		{
			//TODO move this to server-side
			//var costToPay = card.Cost;
			card.Game.Board.Play(card, to, controllingPlayer, stackSrc);

			//if (payCost) controllingPlayer.Pips -= costToPay;
		}

		public static void Move<TCard, TPlayer>(this TCard card, Space to, bool normalMove, TPlayer? mover, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
			=> card.Game.Board.Move(card, to, normalMove, mover, stackSrc);

		public static void Dispel<TCard, TPlayer>(this TCard card, IStackable? stackSrc = null)
			where TCard : class, IGameCard<TCard, TPlayer>
			where TPlayer : IPlayer<TCard, TPlayer>
		{
			card.SetNegated(true, stackSrc);
			card.Discard<TCard, TPlayer>(stackSrc);
		}
	}
}