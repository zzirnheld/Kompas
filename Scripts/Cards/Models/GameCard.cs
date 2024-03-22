using System;
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
using Kompas.Gamestate.Locations.Models;
using Kompas.Gamestate.Players;

namespace Kompas.Cards.Models
{

	public abstract class GameCard<CardType, PlayerType>
		: GameCardBase, IGameCard<CardType, PlayerType>
			where CardType : class, IGameCard<CardType, PlayerType>
			where PlayerType : IPlayer<CardType, PlayerType>
	{
		public abstract ICardController CardController { get; }
		public abstract IGame<CardType, PlayerType> Game { get; }
		IGame IGameCardInfo.Game => Game;

		public int ID { get; private set; }
		public abstract CardType Card { get; }
		IGameCard IGameCardInfo.Card => Card;

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

		public override bool Summoned => Type != 'C' || Location == Location.Board;
		public virtual bool CanRemove => true;
		public virtual int CombatDamage => W;
		#endregion stats

		#region positioning
		private Space? _position;
		public override Space? Position
		{
			get => _position;
			set
			{
				if (null != value) GD.Print($"Position of {CardName} set to {value}");

				_position = value;
				LocationChanged?.Invoke(this, Position);
				//Note: this used to call SetPhysicalLocation. BoardController should handle setting the position of the card in all such cases.
				//Similarly, when a card takes on an augment, the CardController should set the new parent, etc.
				foreach (var aug in augmentsList) aug.Position = value;
			}
		}

		public int IndexInList => LocationModel?.IndexOf(Card) ?? -1;
		public bool InHiddenLocation => IGame.IsHiddenLocation(Location);

		public override IEnumerable<CardType> AdjacentCards
			=> Game?.Board.CardsAdjacentTo(Position) ?? new List<CardType>();
		#endregion positioning

		#region Augments
		private readonly List<CardType> augmentsList = new();
		public IEnumerable<CardType> Augments
		{
			get => augmentsList;
			protected set
			{
				augmentsList.Clear();
				augmentsList.AddRange(value);
			}
		}
		IEnumerable<IGameCard> IGameCardInfo.Augments => Augments;

		private CardType? augmentedCard;
		public CardType? AugmentedCard
		{
			get => augmentedCard;
			set
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
		IGameCard? IGameCardInfo.AugmentedCard => AugmentedCard;

		#endregion

		#region effects
		public abstract IReadOnlyCollection<Effect> Effects { get; }
		#endregion effects

		//movement
		public override int SpacesMoved { get; set; } = 0;

		public virtual int AttacksThisTurn { get; set; }

		//restrictions
		public override IMovementRestriction MovementRestriction { get; }
		public override IRestriction<IGameCardInfo> AttackingDefenderRestriction { get; }
		public override IPlayRestriction PlayRestriction { get; }

		//controller/owners
		public IPlayer ControllingPlayer { get; set; }
		public PlayerType OwningPlayer { get; } //TODO hoist to superclass, this never changes after card construction
		IPlayer IGameCard.OwningPlayer => OwningPlayer;
		public int ControllingPlayerIndex => ControllingPlayer?.Index ?? 0;
		public int OwnerIndex => OwningPlayer?.Index ?? -1;

		private string _bbCodeEffText = string.Empty;
		public override string BBCodeEffText => _bbCodeEffText;

		//misc
		private Location location;
		public override Location Location
		{
			get => location;
			protected set
			{
				location = value;
				LocationChanged?.Invoke(this, Position);
				GD.Print($"Card {ID} named {CardName} location set to {Location}");
			}
		}

		private ILocationModel<CardType, PlayerType> locationModel = Nowhere<CardType, PlayerType>.Instance;
		public ILocationModel<CardType, PlayerType> LocationModel
		{
			get => locationModel;
			set
			{
				GD.Print($"{CardName} moving from {locationModel} to {value}");
				locationModel = value;
				Location = value.Location;
			}
		}
		ILocationModel IGameCard.LocationModel => LocationModel;

		public string BaseJson => CardRepository.GetJsonFromName(CardName)
			?? throw new System.NullReferenceException($"{CardName} doesn't have an associated json?");

		public int TurnsOnBoard { get; set; }

		public GameCardLinksModel CardLinkHandler { get; private set; }

		public EventHandler<Space?>? LocationChanged;

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.Append(base.ToString());
			sb.Append($", ID={ID}, Controlled by {ControllingPlayerIndex}, Owned by {OwnerIndex}, In Location {location}, Position {Position}, ");
			if (AugmentedCard != null) sb.Append($"Augmenting {AugmentedCard.CardName} ID={AugmentedCard.ID}, ");
			if (Augments.Any()) sb.Append($"Augments are {string.Join(", ", Augments.Select(c => $"{c.CardName} ID={c.ID}"))}");
			return sb.ToString();
		}

		protected GameCard(SerializableCard serializeableCard, int id, PlayerType owningPlayer)
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

			var initContext = new EffectInitializationContext(owningPlayer.Game, this); //Can't use property because constructor hasn't gotten there yet

			MovementRestriction = serializeableCard.movementRestriction ?? IMovementRestriction.CreateDefault();
			MovementRestriction.Initialize(initContext);

			AttackingDefenderRestriction = serializeableCard.attackingDefenderRestriction ?? IAttackingDefender.CreateDefault();
			AttackingDefenderRestriction.Initialize(initContext);

			PlayRestriction = serializeableCard.PlayRestriction ?? IPlayRestriction.CreateDefault();
			PlayRestriction.Initialize(initContext);

			GD.Print($"Finished setting up info for card {CardName}");

			UpdateBBCodeEffectText(EffText);
			EffTextChanged += (_, effText) => UpdateBBCodeEffectText(effText);
		}

		private void UpdateBBCodeEffectText(string effText)
		{
			_bbCodeEffText = OwningPlayer.Game.CardRepository.AddKeywordHints(effText);
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
		public void CountSpacesMovedTo(Space to)
		{
			var from = Position ?? throw new InvalidOperationException("Can't count spaces moved while not on board!");
			int cost = MovementRestriction.GetMovementCost(from, to, Game);
			SpacesMoved += cost;
			GD.Print($"Moving {this} to {to} cost {cost}");
		}

		#region augments

		public virtual void AddAugment(CardType augment, IStackable? stackSrc = null)
		{
			//can't add a null augment
			if (augment == null)
				throw new NullAugmentException(stackSrc, this, "Can't add a null augment");
			if (Location != Location.Board)
				throw new CardNotHereException(Location.Board, this, $"Can't put an augment on a card not in {Location}!");

			GD.Print($"Attaching {augment.CardName} from {augment.Location} to {CardName} in {Location}");

			augment.Remove(stackSrc);

			augmentsList.Add(augment);
			augment.AugmentedCard = Card;
		}

		protected virtual void Detach(IStackable? stackSrc = null)
		{
			if (AugmentedCard == null) throw new NotAugmentingException(this);

			AugmentedCard.RemoveAugment(Card);
			AugmentedCard = null;
		}

		//TODO see if any way to narrow down - perhaps making the type param tighter that CardType must be a GameCard<type, type> ?
		public void RemoveAugment(CardType augment) => augmentsList.Remove(augment);
		#endregion augments

		#region statfuncs
		public override void SetN(int n, IStackable? stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetN(n, stackSrc, onlyStatBeingSet);
			//TODO leverage onlyStatBeingSet to only call refresh when necessary. (Will require bookkeeping)
			CardController.RefreshStats();
		}

		public override void SetE(int e, IStackable? stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetE(e, stackSrc, onlyStatBeingSet);
			CardController.RefreshStats();
		}

		public override void SetS(int s, IStackable? stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetS(s, stackSrc, onlyStatBeingSet);
			CardController.RefreshStats();
		}

		public override void SetW(int w, IStackable? stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetW(w, stackSrc, onlyStatBeingSet);
			CardController.RefreshStats();
		}

		public override void SetC(int c, IStackable? stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetC(c, stackSrc, onlyStatBeingSet);
			CardController.RefreshStats();
		}

		public override void SetA(int a, IStackable? stackSrc, bool onlyStatBeingSet = true)
		{
			base.SetA(a, stackSrc, onlyStatBeingSet);
			CardController.RefreshStats();
		}

		/// <summary>
		/// Inflicts the given amount of damage. Used by attacks and (rarely) by effects.
		/// </summary>
		public virtual void TakeDamage(int dmg, IStackable? stackSrc = null)
		{
			if (Location == Location.Board) SetE(E - dmg, stackSrc: stackSrc);
		}

		public virtual void SetNegated(bool negated, IStackable? stackSrc = null) => Negated = negated;
		public virtual void SetActivated(bool activated, IStackable? stackSrc = null) => Activated = activated;
		#endregion statfuncs

		#region moveCard
		/// <summary>
		/// Removes the card from its current location
		/// </summary>
		/// <param name="stackSrc">The stackable (if any) that caused the card's game location to change</param>
		/// <returns><see langword="true"/> if the card was successfully removed, 
		/// <see langword="false"/> if the card is an avatar that got sent back</returns>
		public virtual void Remove(IStackable? stackSrc = null)
		{
			if (Location == Location.Nowhere) return;

			if (AugmentedCard != null) Detach(stackSrc);
			else LocationModel.Remove(Card);
		}

		public virtual void Reveal(IStackable? stackSrc = null)
		{
			//Reveal should only succeed if the card is not known to the enemy
			if (KnownToEnemy) throw new AlreadyKnownException(this);
		}
		#endregion moveCard
	}
}