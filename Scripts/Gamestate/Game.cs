using Kompas.Cards.Loading;

namespace Kompas.Gamestate
{
	public abstract class Game
	{
		public const string CardListPath = "Card Jsons/Card List";

		// The list of locations where cards generally shouldn't be made visible to the opponent
		public static readonly CardLocation[] HiddenLocations =
			new CardLocation[] { CardLocation.Nowhere, CardLocation.Deck, CardLocation.Hand };

		//other scripts
		//public abstract UIController UIController { get; }
		public abstract Settings Settings { get; }

		//game mechanics
		public abstract BoardController BoardController { get; }

		public abstract Player[] Players { get; }
		public int TurnPlayerIndex { get; protected set; } = 0;
		public Player TurnPlayer => Players[TurnPlayerIndex];
		public int FirstTurnPlayer { get; protected set; }

		//game data
		public abstract CardRepository CardRepository { get; }
		public abstract IReadOnlyCollection<GameCard> Cards { get; }
		public int RoundCount { get; protected set; } = 1;
		public virtual int TurnCount { get; protected set; } = 1;
		public virtual int Leyload { get; set; } = 1;

		public abstract GameCard GetCardWithID(int id);

		public virtual IStackable CurrStackEntry => null;
		public abstract IEnumerable<IStackable> StackEntries { get; }
		public abstract bool NothingHappening { get; }

		//game mechanics
		public static bool IsHiddenLocation(CardLocation l) => HiddenLocations.Contains(l);

		public bool BoardHasCopyOf(GameCard card)
			=> Cards.Any(c => c != card && c.Location == CardLocation.Board && c.Controller == card.Controller && c.CardName == card.CardName);

		public bool IsValidSpellSpaceFor(GameCard card, Space space) => BoardController.ValidSpellSpaceFor(card, space);

		public bool IsValidStandardPlaySpace(Space space, Player player)
		{
			/*Debug.Log($"Checking whether player {player?.index} can play a card to {space}. Cards adjacent to that space are" +
				$"{string.Join(",", space.AdjacentSpaces.Select(BoardController.GetCardAt).Where(c => c != null).Select(c => c.CardName))}");*/

			bool cardAtSpaceIsFriendly(GameCardBase card)
			{
				bool isFriendly = card?.Controller == player;
				//if (isFriendly) Debug.Log($"{card} is at {card?.Position} adjacent and friendy to {space}");
				return isFriendly;
			}

			bool existsFriendlyAdjacent(Space adjacentTo)
				=> adjacentTo.AdjacentSpaces.Any(s => cardAtSpaceIsFriendly(BoardController.GetCardAt(s)));

			//first see if there's an adjacent friendly card to this space
			if (existsFriendlyAdjacent(space)) return true;
			//if there isn't, check if the player is Surrounded
			//A player can play to any space if there isn't a space that is adjacent to a friendly card
			else
			{
				bool surrounded = !Space.Spaces.Any(s => BoardController.IsEmpty(s) && existsFriendlyAdjacent(s));
				if (surrounded) Debug.Log($"{player} is surrounded!");
				return surrounded;
			}
		}

		public bool ExistsEffectPlaySpace(IPlayRestriction restriction, Effect eff)
			=> Space.Spaces.Any(s => restriction.IsValid((s, eff.Controller), eff.CurrentResolutionContext));


		protected void ResetCardsForTurn()
		{
			foreach (var c in Cards) c.ResetForTurn(TurnPlayer);
		}

		public virtual bool IsCurrentTarget(GameCard card) => false;
		public virtual bool IsValidTarget(GameCard card) => false;

		public virtual CardBase FocusedCard => null;
	}
}