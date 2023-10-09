using Godot;
using System.Collections.Generic;
using System.Linq;
using Kompas.Cards.Loading;
using Kompas.Cards.Models;
using Kompas.Effects.Models;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;
using Kompas.Shared;
using Kompas.Gamestate.Locations.Models;

namespace Kompas.Gamestate
{
	public static class GameExtensions
	{
		public static bool BoardHasCopyOf(this IGame game, GameCard card)
			=> game.Board.Cards.Any(copy
				=> copy?.Location == Location.Board
				&& copy.ControllingPlayer == card.ControllingPlayer
				&& copy.CardName == card.CardName);

		public static bool IsValidSpellSpaceFor(this IGame game, GameCard card, Space space)
			=> game.Board.ValidSpellSpaceFor(card, space);

		public static bool IsValidStandardPlaySpace(this IGame game, Space space, IPlayer player)
		{
			/*GD.Print($"Checking whether player {player?.index} can play a card to {space}. Cards adjacent to that space are" +
				$"{string.Join(",", space.AdjacentSpaces.Select(BoardController.GetCardAt).Where(c => c != null).Select(c => c.CardName))}");*/

			bool cardAtSpaceIsFriendly(IGameCard card)
			{
				bool isFriendly = card?.ControllingPlayer == player;
				//if (isFriendly) GD.Print($"{card} is at {card?.Position} adjacent and friendy to {space}");
				return isFriendly;
			}

			bool existsFriendlyAdjacent(Space adjacentTo)
				=> adjacentTo.AdjacentSpaces.Any(s => cardAtSpaceIsFriendly(game.Board.GetCardAt(s)));

			//first see if there's an adjacent friendly card to this space
			if (existsFriendlyAdjacent(space)) return true;
			//if there isn't, check if the player is Surrounded
			//A player can play to any space if there isn't a space that is adjacent to a friendly card
			else
			{
				bool surrounded = !Space.Spaces.Any(s => game.Board.IsEmpty(s) && existsFriendlyAdjacent(s));
				if (surrounded) GD.Print($"{player} is surrounded!");
				return surrounded;
			}
		}
	}

	public interface IGame
	{
		public const string CardListPath = "Card Jsons/Card List";

		// The list of locations where cards generally shouldn't be made visible to the opponent
		public static readonly Location[] HiddenLocations =
			new Location[] { Location.Nowhere, Location.Deck, Location.Hand };

		//other scripts
		public GameController GameController { get; }
		public Settings Settings { get; }

		//game mechanics
		public Board Board { get; }

		public IPlayer[] Players { get; }
		public IPlayer TurnPlayer { get; }
		public int FirstTurnPlayer { get; }

		//game data
		public CardRepository CardRepository { get; }
		public IReadOnlyCollection<GameCard> Cards { get; }

		/// <summary>
        /// Number of rounds that have started since the game began.
        /// A round is the first turn player's turn through all other players' turns, until it gets back to the player who went first.
        /// Right after the game begins, this should be 1.
        /// </summary>
		public int RoundCount { get; }
		/// <summary>
        /// The number of turns that have started since the game began.
        /// Right after the game begins, this should be 1.
        /// </summary>
		public int TurnCount { get; }
		/// <summary>
        /// Usually is the same as the TurnCount, but can be inflated further by card effects
        /// </summary>
		public int Leyload { get; set; }

		public GameCard LookupCardByID(int id);

		public IStackController StackController { get; }

		//game mechanics
		public static bool IsHiddenLocation(Location l) => HiddenLocations.Contains(l);
	}

	public interface IStackController
	{
		public IStackable CurrStackEntry { get; }
		public IEnumerable<IStackable> StackEntries { get; }
		public bool NothingHappening { get; }
	}
}