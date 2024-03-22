using System.Collections.Generic;
using Godot;
using Kompas.Effects.Models.Restrictions;
using Kompas.Gamestate;
using Kompas.Gamestate.Locations;
using Kompas.Gamestate.Players;

namespace Kompas.Cards.Models
{
	/// <summary>
	/// Something that has all the same information as a card in a game,
	/// but isn't necessarily the actual card
	/// </summary>
	public interface IGameCardInfo : ICard
	{
		public IGameCard Card { get; }
		public IGame Game { get; }

		public string FileName { get; }

		public int IndexInList { get; }
		public Location Location { get; }
		public Space? Position { get; }

		public string[] SpellSubtypes { get; }
		public int Radius { get; }
		public int Duration { get; }

		public bool Activated { get; }
		public bool Negated { get; }
		public bool Unique { get; }
		public bool KnownToEnemy { get; }
		public bool IsAvatar { get; }
		public bool Summoned { get; }
		
		public int BaseN { get; }
		public int BaseE { get; }
		public int BaseS { get; }
		public int BaseW { get; }
		public int BaseC { get; }
		public int BaseA { get; }
		public bool Hurt => Type == 'C' && Location == Location.Board && E < BaseE;

		public IEnumerable<IGameCard> Augments { get; }
		public IGameCard? AugmentedCard { get; }
		public int SpacesCanMove { get; }

		public IPlayRestriction PlayRestriction { get; }
		public IMovementRestriction MovementRestriction { get; }
		public IRestriction<IGameCardInfo> AttackingDefenderRestriction { get; }

		public IPlayer ControllingPlayer { get; }
		public Space? SubjectivePosition
			=> Position == null ? null : ControllingPlayer.SubjectiveCoords(Position);
	}
}