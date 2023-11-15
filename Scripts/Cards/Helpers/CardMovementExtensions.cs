using Kompas.Cards.Models;
using Kompas.Client.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate;
using Kompas.Gamestate.Players;

namespace Kompas.Cards.Movement
{
	public static class GameCardMovementExtensions
	{
		public static void Discard(this GameCard card, IStackable? stackSrc = null)
			=> card.ControllingPlayer.Discard.Add(card, stackableCause: stackSrc);

		public static void Hand(this GameCard card, IPlayer controllingPlayer, IStackable? stackSrc = null)
			=> controllingPlayer.Hand.Add(card, stackableCause: stackSrc);
		public static void Rehand(this GameCard card, IStackable? stackSrc = null) => card.Hand(card.OwningPlayer, stackSrc);

		public static void Reshuffle(this GameCard card, IPlayer controllingPlayer, IStackable? stackSrc = null)
			=> controllingPlayer.Deck.ShuffleIn(card, stackSrc);
		public static void Reshuffle(this GameCard card, IStackable? stackSrc = null) => card.Reshuffle(card.OwningPlayer, stackSrc);

		public static void Topdeck(this GameCard card, IPlayer controllingPlayer, IStackable? stackSrc = null)
			=> controllingPlayer.Deck.PushTopdeck(card, stackSrc);
		public static void Topdeck(this GameCard card, IStackable? stackSrc = null) => card.Topdeck(card.OwningPlayer, stackSrc);

		public static void Bottomdeck(this GameCard card, IPlayer controllingPlayer, IStackable? stackSrc = null)
			=> controllingPlayer.Deck.PushBottomdeck(card, stackSrc);
		public static void Bottomdeck(this GameCard card, IStackable? stackSrc = null) => card.Bottomdeck(card.OwningPlayer, stackSrc);

		public static void Annihilate(this GameCard card, IPlayer controllingPlayer, IStackable? stackSrc = null)
			=> controllingPlayer.Annihilation.Add(card, stackableCause: stackSrc);
		public static void Annihilate(this GameCard card, IStackable? stackSrc = null) => card.Annihilate(card.OwningPlayer, stackSrc);

		//If you're looking for this, you'll need to do it more manually by the client/server version of the thing, to maintain the knowledge of what kind of controller you need
		public static void Play(this GameCard card, Space to, IPlayer controllingPlayer, IStackable? stackSrc = null, bool payCost = false)
		{
			//TODO move this to server-side
			//var costToPay = card.Cost;
			card.Game.Board.Play(card, to, controllingPlayer, stackSrc);

			//if (payCost) controllingPlayer.Pips -= costToPay;
		}

		public static void Move(this GameCard card, Space to, bool normalMove, IPlayer mover, IStackable? stackSrc = null)
			=> card.Game.Board.Move(card, to, normalMove, mover, stackSrc);

		public static void Dispel(this GameCard card, IStackable? stackSrc = null)
		{
			card.SetNegated(true, stackSrc);
			card.Discard(stackSrc);
		}
	}
}