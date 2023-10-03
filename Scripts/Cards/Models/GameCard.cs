using System.Collections.Generic;
using System.Linq;
using System.Text;
using Godot;
using Kompas.Cards.Controllers;
using Kompas.Cards.Loading;
using Kompas.Effects.Models;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Gamestate.Exceptions;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;

namespace Kompas.Cards.Models
{
	public abstract class GameCard : GameCardBase, IGameCard
	{
		public abstract CardController CardController { get; }
		public abstract IGame Game { get; }

		public int ID { get; private set; }
		public GameCard Card => this;

		protected SerializableCard InitialCardValues { get; private set; }

		#region stats
		public override int BaseN => InitialCardValues?.n ?? default;
		public override int BaseE => InitialCardValues?.e ?? default;
		public override int BaseS => InitialCardValues?.s ?? default;
		public override int BaseW => InitialCardValues?.w ?? default;
		public override int BaseC => InitialCardValues?.c ?? default;
		public override int BaseA => InitialCardValues?.a ?? default;

		public int Negations { get; private set; } = 0;
		public override bool Negated
		{
			get => Negations > 0;
			protected set
			{
				if (value) Negations++;
				else Negations--;

				foreach (var e in Effects) e.Negated = Negated;
			}
		}
		public int Activations { get; private set; } = 0;
		public override bool Activated
		{
			get => Activations > 0;
			protected set
			{
				if (value) Activations++;
				else Activations--;
			}
		}

		public override bool Summoned => CardType != 'C' || Location == Location.Board;
		public virtual bool CanRemove => true;
		public virtual int CombatDamage => W;
		#endregion stats

		#region positioning
		private Space position;
		public override Space Position
		{
			get => position;
			set
			{
				if (null != value) GD.Print($"Position of {CardName} set to {value}");

				position = value;
				CardController?.SetPhysicalLocation(Location);
				foreach (var aug in augmentsList) aug.Position = value;
			}
		}

		public int IndexInList => LocationModel?.IndexOf(this) ?? -1;
		public bool InHiddenLocation => IGame.IsHiddenLocation(Location);

		public override IReadOnlyCollection<GameCard> AdjacentCards
			=> Game?.Board.CardsAdjacentTo(Position) ?? new List<GameCard>();
		#endregion positioning

		#region Augments
		private readonly List<GameCard> augmentsList = new();
		public override IReadOnlyCollection<GameCard> Augments
		{
			get => augmentsList;
			protected set
			{
				augmentsList.Clear();
				augmentsList.AddRange(value);
			}
		}

		private GameCard augmentedCard;
		public override GameCard AugmentedCard
		{
			get => augmentedCard;
			protected set
			{
				GD.Print($"{CardName} augmenting {augmentedCard} will now be augmenting {value}");
				augmentedCard = value;
				if (augmentedCard != null)
				{
					LocationModel = augmentedCard.LocationModel;
					Position = augmentedCard.Position;
				}
			}
		}

		public bool Attached => AugmentedCard != null;
		#endregion

		#region effects
		public abstract IReadOnlyCollection<Effect> Effects { get; }
		#endregion effects

		//movement
		public override int SpacesMoved { get; set; } = 0;

		public virtual int AttacksThisTurn { get; set; }

		//restrictions
		public override IMovementRestriction MovementRestriction { get; }
		public override IRestriction<IGameCard> AttackingDefenderRestriction { get; }
		public override IPlayRestriction PlayRestriction { get; }

		//controller/owners
		public IPlayer ControllingPlayer { get; set; }
		public IPlayer OwningPlayer { get; } //TODO hoist to superclass, this never changes after card construction
		public int ControllerIndex => ControllingPlayer?.Index ?? 0;
		public int OwnerIndex => OwningPlayer?.Index ?? -1;

		//misc
		private Location location;
		public override Location Location
		{
			get => location;
			protected set
			{
				location = value;
				GD.Print($"Card {ID} named {CardName} location set to {Location}");
				//TODO: card controller
				//if (CardController != null) CardController.SetPhysicalLocation(Location);
				GD.PrintErr($"Missing a card control. Is this a debug card?");
			}
		}

		private ILocationModel locationModel;
		public ILocationModel LocationModel
		{
			get => locationModel;
			set
			{
				GD.Print($"{CardName} moving from {locationModel} to {value}");
				locationModel = value;
				Location = value.Location;
			}
		}

		public string BaseJson => CardRepository.GetJsonFromName(CardName);

		public int TurnsOnBoard { get; set; }

		public GameCardLinksModel CardLinkHandler { get; private set; }

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append($", ID={ID}, Controlled by {ControllerIndex}, Owned by {OwnerIndex}, In Location {location}, Position {Position}, ");
			if (Attached) sb.Append($"Augmenting {AugmentedCard.CardName} ID={AugmentedCard.ID}, ");
			if (Augments.Count > 0) sb.Append($"Augments are {string.Join(", ", Augments.Select(c => $"{c.CardName} ID={c.ID}"))}");
			return sb.ToString();
		}

		protected GameCard(int id)
			: base(default,
				  string.Empty, System.Array.Empty<string>(),
				  false,
				  0, 0,
				  'C', "Dummy Card", "generic/The Intern",
				  "",
				  "")
		{
			CardLinkHandler = new GameCardLinksModel(this);

			ID = id;
		}

		protected GameCard(SerializableCard serializeableCard, int id, IGame game, IPlayer owningPlayer)
			: base(serializeableCard.Stats,
					   serializeableCard.subtext, serializeableCard.spellTypes,
					   serializeableCard.unique,
					   serializeableCard.radius, serializeableCard.duration,
					   serializeableCard.cardType, serializeableCard.cardName, CardRepository.FileNameFor(serializeableCard.cardName),
					   serializeableCard.effText,
					   serializeableCard.subtypeText)
		{
			CardLinkHandler = new GameCardLinksModel(this);

			ControllingPlayer = OwningPlayer = owningPlayer;

			ID = id;
			InitialCardValues = serializeableCard;

			var initContext = new EffectInitializationContext(game, this); //Can't use property because constructor hasn't gotten there yet

			MovementRestriction = serializeableCard.movementRestriction ?? IMovementRestriction.CreateDefault();
			MovementRestriction.Initialize(initContext);

			AttackingDefenderRestriction = serializeableCard.attackingDefenderRestriction ?? IAttackingDefender.CreateDefault();
			AttackingDefenderRestriction.Initialize(initContext);

			PlayRestriction = serializeableCard.PlayRestriction ?? IPlayRestriction.CreateDefault();
			PlayRestriction.Initialize(initContext);

			GD.Print($"Finished setting up info for card {CardName}");
		}

		/// <summary>
		/// Resets anything that needs to be reset for the start of the turn.
		/// </summary>
		public virtual void ResetForTurn(IPlayer turnPlayer)
		{
			foreach (Effect eff in Effects) eff.ResetForTurn(turnPlayer);

			SpacesMoved = 0;
			AttacksThisTurn = 0;
			if (Location == Location.Board) TurnsOnBoard++;
		}

		public void ResetForStack()
		{
			foreach (var e in Effects) e.TimesUsedThisStack = 0;
		}

		/// <summary>
		/// Accumulates the distance to <paramref name="to"/> into the number of spaces this card moved this turn.
		/// </summary>
		/// <param name="to">The space being moved to</param>
		public void CountSpacesMovedTo((int x, int y) to) => SpacesMoved += Game.Board.ShortestEmptyPath(this, to);

		#region augments

		public virtual void AddAugment(GameCard augment, IStackable stackSrc = null)
		{
			//can't add a null augment
			if (augment == null)
				throw new NullAugmentException(stackSrc, this, "Can't add a null augment");
			if (Location != Location.Board)
				throw new CardNotHereException(Location.Board, this, $"Can't put an augment on a card not in {Location}!");

			GD.Print($"Attaching {augment.CardName} from {augment.Location} to {CardName} in {Location}");

			augment.Remove(stackSrc);

			augmentsList.Add(augment);
			augment.AugmentedCard = this;
		}

		protected virtual void Detach(IStackable stackSrc = null)
		{
			if (!Attached) throw new NotAugmentingException(this);

			AugmentedCard.augmentsList.Remove(this);
			AugmentedCard = null;
		}
		#endregion augments

		#region statfuncs
		public override void SetN(int n, IStackable stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetN(n, stackSrc, onlyStatBeingSet);
			//TODO leverage onlyStatBeingSet to only call refresh when necessary. (Will require bookkeeping)
			CardController.RefreshStats();
		}

		public override void SetE(int e, IStackable stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetE(e, stackSrc, onlyStatBeingSet);
			CardController.RefreshStats();
		}

		public override void SetS(int s, IStackable stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetS(s, stackSrc, onlyStatBeingSet);
			CardController.RefreshStats();
		}

		public override void SetW(int w, IStackable stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetW(w, stackSrc, onlyStatBeingSet);
			CardController.RefreshStats();
		}

		public override void SetC(int c, IStackable stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetC(c, stackSrc, onlyStatBeingSet);
			CardController.RefreshStats();
		}

		public override void SetA(int a, IStackable stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetA(a, stackSrc, onlyStatBeingSet);
			CardController.RefreshStats();
		}

		/// <summary>
		/// Inflicts the given amount of damage. Used by attacks and (rarely) by effects.
		/// </summary>
		public virtual void TakeDamage(int dmg, IStackable stackSrc = null)
		{
			if (Location == Location.Board) SetE(E - dmg, stackSrc: stackSrc);
		}

		public virtual void SetNegated(bool negated, IStackable stackSrc = null) => Negated = negated;
		public virtual void SetActivated(bool activated, IStackable stackSrc = null) => Activated = activated;
		#endregion statfuncs

		#region moveCard
		/// <summary>
		/// Removes the card from its current location
		/// </summary>
		/// <param name="stackSrc">The stackable (if any) that caused the card's game location to change</param>
		/// <returns><see langword="true"/> if the card was successfully removed, 
		/// <see langword="false"/> if the card is an avatar that got sent back</returns>
		public virtual void Remove(IStackable stackSrc = null)
		{
			if (Location == Location.Nowhere) return;

			if (Attached) Detach(stackSrc);
			else LocationModel.Remove(this);
		}

		public virtual void Reveal(IStackable stackSrc = null)
		{
			//Reveal should only succeed if the card is not known to the enemy
			if (KnownToEnemy) throw new AlreadyKnownException(this);
		}
		#endregion moveCard
	}
}